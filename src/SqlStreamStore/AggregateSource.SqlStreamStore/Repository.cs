using System;
using System.Linq;
using System.Threading;
using AggregateSource;
using SqlStreamStore;
using SqlStreamStore.Streams;
using StreamStoreStore.Json;

namespace SSS
{
    /// <summary>
    /// Represents a virtual collection of <typeparamref name="TAggregateRoot"/>.
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root in this collection.</typeparam>
    public class Repository<TAggregateRoot> : IAsyncRepository<TAggregateRoot> where TAggregateRoot : IAggregateRootEntity
    {
        readonly Func<TAggregateRoot> _rootFactory;
        readonly UnitOfWork _unitOfWork;
        readonly IStreamStore _eventStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="rootFactory">The aggregate root entity factory.</param>
        /// <param name="unitOfWork">The unit of work to interact with.</param>
        /// <param name="eventStore">The event store to use.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="rootFactory"/> or <paramref name="unitOfWork"/> or <paramref name="eventStore"/> is null.</exception>
        public Repository(Func<TAggregateRoot> rootFactory, UnitOfWork unitOfWork, IStreamStore eventStore)
        {
            if (rootFactory == null) throw new ArgumentNullException("rootFactory");
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
            if (eventStore == null) throw new ArgumentNullException("eventStore");
            _rootFactory = rootFactory;
            _unitOfWork = unitOfWork;
            _eventStore = eventStore;
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
        /// Gets the event store to use.
        /// </summary>
        /// <value>
        /// The event store to use.
        /// </value>
        public IStreamStore EventStore
        {
            get { return _eventStore; }
        }

        /// <summary>
        /// Gets the aggregate root entity associated with the specified aggregate identifier.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <returns>An instance of <typeparamref name="TAggregateRoot"/>.</returns>
        /// <exception cref="AggregateNotFoundException">Thrown when an aggregate is not found.</exception>
        public async System.Threading.Tasks.Task<TAggregateRoot> GetAsync(string identifier)
        {
            var result = await GetOptionalAsync(identifier);
            if (!result.HasValue)
                throw new AggregateNotFoundException(identifier, typeof(TAggregateRoot));
            return result.Value;
        }

        /// <summary>
        /// Attempts to get the aggregate root entity associated with the aggregate identifier.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <returns>The found <typeparamref name="TAggregateRoot"/>, or empty if not found.</returns>
        public async System.Threading.Tasks.Task<Optional<TAggregateRoot>> GetOptionalAsync(string identifier)
        {
            Aggregate aggregate;
            if (UnitOfWork.TryGet(identifier, out aggregate))
            {
                return new Optional<TAggregateRoot>((TAggregateRoot) aggregate.Root);
            }
            var page = await EventStore.ReadStreamForwards(identifier, StreamVersion.Start, 100);

            if (page.Status == PageReadStatus.StreamNotFound)
                return Optional<TAggregateRoot>.Empty;

            var events = page.Messages.ToList();

            while (!page.IsEnd)
            {
                events.AddRange(page.Messages);
            }

            var root = RootFactory();
            var eventObjects = events.Select(message =>
            {
                var eventType = Type.GetType(message.Type);
                var eventData = message.GetJsonData().GetAwaiter().GetResult();
                return SimpleJson.DeserializeObject(eventData, eventType);
            });
            root.Initialize(eventObjects);
            UnitOfWork.Attach(new Aggregate(identifier, page.LastStreamVersion, root));
            return new Optional<TAggregateRoot>(root);
        }

        /// <summary>
        /// Adds the aggregate root entity to this collection using the specified aggregate identifier.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <param name="root">The aggregate root entity.</param>
        public void Add(string identifier, TAggregateRoot root)
        {
            UnitOfWork.Attach(new Aggregate(identifier, 0, root));
        }
    }
}
