using System;
using System.Linq;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore {
  public class Repository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : AggregateRootEntity {
    readonly Func<TAggregateRoot> _rootFactory;
    readonly UnitOfWork _unitOfWork;
    readonly EventStoreConnection _connection;
    readonly EventStoreReadConfiguration _configuration;

    public Repository(Func<TAggregateRoot> rootFactory, UnitOfWork unitOfWork, EventStoreConnection connection)
      : this(rootFactory, unitOfWork, connection, EventStoreReadConfiguration.Default) {
    }

    public Repository(Func<TAggregateRoot> rootFactory, UnitOfWork unitOfWork, EventStoreConnection connection, EventStoreReadConfiguration configuration) {
      if (rootFactory == null) throw new ArgumentNullException("rootFactory");
      if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
      if (connection == null) throw new ArgumentNullException("connection");
      if (configuration == null) throw new ArgumentNullException("configuration");
      _rootFactory = rootFactory;
      _unitOfWork = unitOfWork;
      _connection = connection;
      _configuration = configuration;
    }

    public TAggregateRoot Get(Guid id) {
      TAggregateRoot root;
      if (!TryGet(id, out root))
        throw new AggregateNotFoundException(id, typeof(TAggregateRoot));
      return root;
    }

    public bool TryGet(Guid id, out TAggregateRoot root) {
      Aggregate aggregate;
      if (_unitOfWork.TryGet(id, out aggregate)) {
        root = (TAggregateRoot)aggregate.Root;
        return true;
      }
      var stream = StreamName.Create<TAggregateRoot>(id);
      var slice = _connection.ReadStreamEventsForward(stream, 1, _configuration.SliceSize, false);
      if (slice.Status == SliceReadStatus.StreamDeleted || slice.Status == SliceReadStatus.StreamNotFound) {
        root = null;
        return false;
      }
      root = _rootFactory();
      root.Initialize(slice.Events.Select(resolved => _configuration.Deserializer.Deserialize(resolved)));
      while (!slice.IsEndOfStream) {
        slice = _connection.ReadStreamEventsForward(stream, slice.NextEventNumber, _configuration.SliceSize, false);
        root.Initialize(slice.Events.Select(resolved => _configuration.Deserializer.Deserialize(resolved)));
      }
      aggregate = new Aggregate(id, slice.LastEventNumber, root);
      _unitOfWork.Attach(aggregate);
      return true;
    }

    public void Add(Guid id, TAggregateRoot root) {
      _unitOfWork.Attach(new Aggregate(id, ExpectedVersion.NoStream, root));
    }
  }
}
