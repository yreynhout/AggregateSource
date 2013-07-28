using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore
{
    /// <summary>
    /// Represents an asynchronous, virtual collection of <typeparamref name="TAggregateRoot"/>.
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root in this collection.</typeparam>
    public class AsyncRepository<TAggregateRoot> : IAsyncRepository<TAggregateRoot>
        where TAggregateRoot : IAggregateRootEntity
    {
        readonly Func<TAggregateRoot> _rootFactory;
        readonly ConcurrentUnitOfWork _unitOfWork;
        readonly IAsyncEventReader _reader;
        readonly IAsyncEventReader _connection;
        readonly RepositoryConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRepository{TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="rootFactory">The aggregate root entity factory.</param>
        /// <param name="unitOfWork">The unit of work to interact with.</param>
        /// <param name="reader">The event reader.</param>
        /// <param name="configuration">The event store configuration to use.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="rootFactory"/> or <paramref name="unitOfWork"/> or <paramref name="reader"/> or <paramref name="configuration"/> is null.</exception>
        public AsyncRepository(Func<TAggregateRoot> rootFactory, ConcurrentUnitOfWork unitOfWork,
                               IAsyncEventReader reader, RepositoryConfiguration configuration)
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
        public ConcurrentUnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
        }

        /// <summary>
        /// Gets the asynchronous event reader to use.
        /// </summary>
        /// <value>
        /// The asynchronous event reader to use.
        /// </value>
        public IAsyncEventReader Reader
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
        public async Task<TAggregateRoot> GetAsync(string identifier)
        {
            var result = await GetOptionalAsync(identifier);
            if (!result.HasValue)
                throw new AggregateNotFoundException(identifier, typeof (TAggregateRoot));
            return result.Value;
        }

        /// <summary>
        /// Attempts to get the aggregate root entity associated with the aggregate identifier.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <returns>The found <typeparamref name="TAggregateRoot"/>, or empty if not found.</returns>
        public async Task<Optional<TAggregateRoot>> GetOptionalAsync(string identifier)
        {
            Aggregate aggregate;
            if (_unitOfWork.TryGet(identifier, out aggregate))
            {
                return new Optional<TAggregateRoot>((TAggregateRoot) aggregate.Root);
            }

            using (var enumerator = _reader.ReadAsync(identifier, StreamPosition.Start))
            {
                var moved = await enumerator.MoveNextAsync();
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
                        moved = await enumerator.MoveNextAsync();
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