using System;
using System.Linq;
using EventStore.ClientAPI;

namespace AggregateSource.EventStore
{
    /// <summary>
    /// Represents a virtual collection of <typeparamref name="TAggregateRoot"/>.
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root in this collection.</typeparam>
    public class Repository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : IAggregateRootEntity
    {
        readonly Func<TAggregateRoot> _rootFactory;
        readonly UnitOfWork _unitOfWork;
        readonly IEventStoreConnection _connection;
        readonly EventReaderConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="rootFactory">The aggregate root entity factory.</param>
        /// <param name="unitOfWork">The unit of work to interact with.</param>
        /// <param name="connection">The event store connection to use.</param>
        /// <param name="configuration">The event reader configuration to use.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="rootFactory"/> or <paramref name="unitOfWork"/> or <paramref name="connection"/> or <paramref name="configuration"/> is null.</exception>
        public Repository(Func<TAggregateRoot> rootFactory, UnitOfWork unitOfWork, IEventStoreConnection connection,
                          EventReaderConfiguration configuration)
        {
            if (rootFactory == null) throw new ArgumentNullException("rootFactory");
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
            if (connection == null) throw new ArgumentNullException("connection");
            if (configuration == null) throw new ArgumentNullException("configuration");
            _rootFactory = rootFactory;
            _unitOfWork = unitOfWork;
            _connection = connection;
            _configuration = configuration;
        }

        /// <summary>
        /// Gets the aggregate root entity factory.
        /// </summary>
        /// <value>
        /// The aggregate root entity factory.
        /// </value>
        public Func<TAggregateRoot> RootFactory
        {
            get { return _rootFactory; }
        }

        /// <summary>
        /// Gets the unit of work.
        /// </summary>
        /// <value>
        /// The unit of work.
        /// </value>
        public UnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
        }

        /// <summary>
        /// Gets the event store connection to use.
        /// </summary>
        /// <value>
        /// The event store connection to use.
        /// </value>
        public IEventStoreConnection Connection
        {
            get { return _connection; }
        }

        /// <summary>
        /// Gets the event reader configuration.
        /// </summary>
        /// <value>
        /// The event reader configuration.
        /// </value>
        public EventReaderConfiguration Configuration
        {
            get { return _configuration; }
        }

        /// <summary>
        /// Gets the aggregate root entity associated with the specified aggregate identifier.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <returns>An instance of <typeparamref name="TAggregateRoot"/>.</returns>
        /// <exception cref="AggregateNotFoundException">Thrown when an aggregate is not found.</exception>
        public TAggregateRoot Get(string identifier)
        {
            var result = GetOptional(identifier);
            if (!result.HasValue)
                throw new AggregateNotFoundException(identifier, typeof (TAggregateRoot));
            return result.Value;
        }

        /// <summary>
        /// Attempts to get the aggregate root entity associated with the aggregate identifier.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <returns>The found <typeparamref name="TAggregateRoot"/>, or empty if not found.</returns>
        public Optional<TAggregateRoot> GetOptional(string identifier)
        {
            Aggregate aggregate;
            if (_unitOfWork.TryGet(identifier, out aggregate))
            {
                return new Optional<TAggregateRoot>((TAggregateRoot) aggregate.Root);
            }
            var streamUserCredentials = _configuration.StreamUserCredentialsResolver.Resolve(identifier);
            var streamName = _configuration.StreamNameResolver.Resolve(identifier);
            var slice = _connection.
                ReadStreamEventsForwardAsync(
                    streamName, StreamPosition.Start, _configuration.SliceSize, false, streamUserCredentials).
                Result;
            if (slice.Status == SliceReadStatus.StreamDeleted || slice.Status == SliceReadStatus.StreamNotFound)
            {
                return Optional<TAggregateRoot>.Empty;
            }
            var root = _rootFactory();
            root.Initialize(slice.Events.SelectMany(resolved => _configuration.Deserializer.Deserialize(resolved)));
            while (!slice.IsEndOfStream)
            {
                slice = _connection.
                    ReadStreamEventsForwardAsync(
                        streamName, slice.NextEventNumber, _configuration.SliceSize, false, streamUserCredentials).
                    Result;
                root.Initialize(slice.Events.SelectMany(resolved => _configuration.Deserializer.Deserialize(resolved)));
            }
            aggregate = new Aggregate(identifier, slice.LastEventNumber, root);
            _unitOfWork.Attach(aggregate);
            return new Optional<TAggregateRoot>(root);
        }

        /// <summary>
        /// Adds the aggregate root entity to this collection using the specified aggregate identifier.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <param name="root">The aggregate root entity.</param>
        public void Add(string identifier, TAggregateRoot root)
        {
            _unitOfWork.Attach(new Aggregate(identifier, ExpectedVersion.NoStream, root));
        }
    }
}