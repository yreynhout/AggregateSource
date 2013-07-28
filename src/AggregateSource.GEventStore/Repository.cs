using System;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore
{
    /// <summary>
    /// Represents a virtual collection of <typeparamref name="TAggregateRoot"/>.
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root in this collection.</typeparam>
    public class Repository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : IAggregateRootEntity
    {
        readonly Func<TAggregateRoot> _rootFactory;
        readonly UnitOfWork _unitOfWork;
        readonly IEventReader _reader;
        readonly RepositoryConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="rootFactory">The aggregate root entity factory.</param>
        /// <param name="unitOfWork">The unit of work to interact with.</param>
        /// <param name="reader">The event reader.</param>
        /// <param name="configuration">The repository configuration.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="rootFactory"/> or <paramref name="unitOfWork"/> or <paramref name="reader"/> or <paramref name="configuration"/> is <c>null</c>.</exception>
        public Repository(Func<TAggregateRoot> rootFactory, UnitOfWork unitOfWork, IEventReader reader, RepositoryConfiguration configuration)
        {
            if (rootFactory == null) throw new ArgumentNullException("rootFactory");
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
            if (reader == null) throw new ArgumentNullException("reader");
            if (configuration == null) throw new ArgumentNullException("configuration");
            _rootFactory = rootFactory;
            _unitOfWork = unitOfWork;
            _reader = reader;
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
        /// Gets the event reader.
        /// </summary>
        /// <value>
        /// The event reader.
        /// </value>
        public IEventReader EventReader
        {
            get { return _reader; }
        }

        /// <summary>
        /// Gets the repository configuration.
        /// </summary>
        /// <value>
        /// The repository configuration.
        /// </value>
        public RepositoryConfiguration Configuration
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
            using (var enumerator = _reader.Read(identifier, StreamPosition.Start).GetEnumerator())
            {
                var moved = enumerator.MoveNext();
                if (moved)
                {
                    var root = _rootFactory();
                    EventsSlice slice;
                    do
                    {
                        slice = enumerator.Current;
                        if (slice.Status == SliceReadStatus.StreamDeleted)
                        {
                            return Optional<TAggregateRoot>.Empty;
                        }
                        if (slice.Status == SliceReadStatus.StreamNotFound && _configuration.RequireStream)
                        {
                            return Optional<TAggregateRoot>.Empty;
                        }
                        root.Initialize(slice.Events);
                        moved = enumerator.MoveNext();
                    } while (moved);
                    aggregate = slice.Status == SliceReadStatus.StreamNotFound // && !_configuration.RequireStream
                                    ? new Aggregate(identifier, ExpectedVersion.NoStream, root)
                                    : new Aggregate(identifier, slice.LastEventNumber, root);
                    _unitOfWork.Attach(aggregate);
                    return new Optional<TAggregateRoot>(root);
                }
                return Optional<TAggregateRoot>.Empty;
            }
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