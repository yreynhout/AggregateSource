using System;
using System.Linq;
using NEventStore;

namespace AggregateSource.NEventStore.Snapshots
{
    /// <summary>
    /// Represents a virtual collection of <typeparamref name="TAggregateRoot"/>.
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root in this collection.</typeparam>
    public class SnapshotableRepository<TAggregateRoot> : IRepository<TAggregateRoot>
        where TAggregateRoot : IAggregateRootEntity, ISnapshotable
    {
        readonly Func<TAggregateRoot> _rootFactory;
        readonly UnitOfWork _unitOfWork;
        readonly IStoreEvents _eventStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotableRepository{TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="rootFactory">The aggregate root entity factory.</param>
        /// <param name="unitOfWork">The unit of work to interact with.</param>
        /// <param name="eventStore">The event store to use.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="rootFactory"/> or <paramref name="unitOfWork"/> or <paramref name="eventStore"/> is null.</exception>
        public SnapshotableRepository(Func<TAggregateRoot> rootFactory, UnitOfWork unitOfWork, IStoreEvents eventStore)
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
        public IStoreEvents Store
        {
            get { return _eventStore; }
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
            var snapshot = _eventStore.Advanced.GetSnapshot(identifier, Int32.MaxValue);
            if (snapshot != null)
            {
                using (var stream = _eventStore.OpenStream(snapshot, Int32.MaxValue))
                {
                    var root = _rootFactory();
                    root.RestoreSnapshot(snapshot.Payload);
                    root.Initialize(stream.CommittedEvents.Select(eventMessage => eventMessage.Body));
                    _unitOfWork.Attach(new Aggregate(identifier, stream.StreamRevision, root));
                    return new Optional<TAggregateRoot>(root);
                }
            }
            using (var stream = _eventStore.OpenStream(identifier, minRevision: 0))
            {
                if (stream.StreamRevision == 0)
                    return Optional<TAggregateRoot>.Empty;

                var root = _rootFactory();
                root.Initialize(stream.CommittedEvents.Select(eventMessage => eventMessage.Body));
                _unitOfWork.Attach(new Aggregate(identifier, stream.StreamRevision, root));
                return new Optional<TAggregateRoot>(root);
            }
        }

        /// <summary>
        /// Adds the aggregate root entity to this collection using the specified aggregate identifier.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <param name="root">The aggregate root entity.</param>
        public void Add(string identifier, TAggregateRoot root)
        {
            _unitOfWork.Attach(new Aggregate(identifier, 0, root));
        }
    }
}