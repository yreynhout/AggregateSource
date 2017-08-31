using System;
using System.Linq;
using System.Threading.Tasks;
using AggregateSource;
using SqlStreamStore;

namespace SSS.Snapshots
{
    /// <summary>
    /// Represents a virtual collection of <typeparamref name="TAggregateRoot"/>.
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root in this collection.</typeparam>
    public class SnapshotableRepository<TAggregateRoot> : IRepository<TAggregateRoot>
        where TAggregateRoot : IAggregateRootEntity, ISnapshotable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotableRepository{TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="rootFactory">The aggregate root entity factory.</param>
        /// <param name="unitOfWork">The unit of work to interact with.</param>
        /// <param name="eventStore">The event store to use.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="rootFactory"/> or <paramref name="unitOfWork"/> or <paramref name="eventStore"/> is null.</exception>
        public SnapshotableRepository(Func<TAggregateRoot> rootFactory, UnitOfWork unitOfWork, IStreamStore eventStore)
        {
            RootFactory = rootFactory ?? throw new ArgumentNullException(nameof(rootFactory));
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            Store = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        }

        /// <summary>
        /// Gets the aggregate root entity factory.
        /// </summary>
        /// <value>
        /// The aggregate root entity factory.
        /// </value>
        public Func<TAggregateRoot> RootFactory { get; }

        /// <summary>
        /// Gets the unit of work.
        /// </summary>
        /// <value>
        /// The unit of work.
        /// </value>
        public UnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Gets the event store to use.
        /// </summary>
        /// <value>
        /// The event store to use.
        /// </value>
        public IStreamStore Store { get; }

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
        public async System.Threading.Tasks.Task<Optional<TAggregateRoot>> GetOptionalAsync(string identifier)
        {
            Aggregate aggregate;
            if (UnitOfWork.TryGet(identifier, out aggregate))
            {
                return new Optional<TAggregateRoot>((TAggregateRoot) aggregate.Root);
            }

            var snapshot = Store.Advanced.GetSnapshot(identifier, Int32.MaxValue);
            if (snapshot != null)
            {
                using (var stream = Store.OpenStream(snapshot, Int32.MaxValue))
                {
                    var root = RootFactory();
                    root.RestoreSnapshot(snapshot.Payload);
                    root.Initialize(stream.CommittedEvents.Select(eventMessage => eventMessage.Body));
                    UnitOfWork.Attach(new Aggregate(identifier, stream.StreamRevision, root));
                    return new Optional<TAggregateRoot>(root);
                }
            }
            using (var stream = Store.OpenStream(identifier, minRevision: 0))
            {
                if (stream.StreamRevision == 0)
                    return Optional<TAggregateRoot>.Empty;

                var root = RootFactory();
                root.Initialize(stream.CommittedEvents.Select(eventMessage => eventMessage.Body));
                UnitOfWork.Attach(new Aggregate(identifier, stream.StreamRevision, root));
                return new Optional<TAggregateRoot>(root);
            }

            //Aggregate aggregate;
            //if (UnitOfWork.TryGet(identifier, out aggregate))
            //{
            //    return new Optional<TAggregateRoot>((TAggregateRoot)aggregate.Root);
            //}
            //var page = await EventStore.ReadStreamForwards(identifier, StreamVersion.Start, 100);

            //if (page.Status == PageReadStatus.StreamNotFound)
            //    return Optional<TAggregateRoot>.Empty;

            //var events = page.Messages.ToList();

            //while (!page.IsEnd)
            //{
            //    events.AddRange(page.Messages);
            //}

            //var root = RootFactory();
            //root.Initialize(events.Select(message => message.GetJsonDataAs<object>()));
            //UnitOfWork.Attach(new Aggregate(identifier, page.LastStreamVersion, root));
            //return new Optional<TAggregateRoot>(root);
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