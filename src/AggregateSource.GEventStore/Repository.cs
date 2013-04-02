using System;
using System.Linq;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore {
  public class Repository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : IAggregateRootEntity {
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
      var result = GetOptional(id);
      if (!result.HasValue)
        throw new AggregateNotFoundException(id, typeof(TAggregateRoot));
      return result.Value;
    }

    public Optional<TAggregateRoot> GetOptional(Guid id) {
      Aggregate aggregate;
      if (_unitOfWork.TryGet(id, out aggregate)) {
        return new Optional<TAggregateRoot>((TAggregateRoot)aggregate.Root);
      }
      var stream = StreamName.Create<TAggregateRoot>(id);
      var slice = _connection.ReadStreamEventsForward(stream, 1, _configuration.SliceSize, false);
      if (slice.Status == SliceReadStatus.StreamDeleted || slice.Status == SliceReadStatus.StreamNotFound) {
        return Optional<TAggregateRoot>.Empty;
      }
      var root = _rootFactory();
      root.Initialize(slice.Events.Select(resolved => _configuration.Deserializer.Deserialize(resolved)));
      while (!slice.IsEndOfStream) {
        slice = _connection.ReadStreamEventsForward(stream, slice.NextEventNumber, _configuration.SliceSize, false);
        root.Initialize(slice.Events.Select(resolved => _configuration.Deserializer.Deserialize(resolved)));
      }
      aggregate = new Aggregate(id, slice.LastEventNumber, root);
      _unitOfWork.Attach(aggregate);
      return new Optional<TAggregateRoot>(root);
    }

    public void Add(Guid id, TAggregateRoot root) {
      _unitOfWork.Attach(new Aggregate(id, ExpectedVersion.NoStream, root));
    }
  }
}
