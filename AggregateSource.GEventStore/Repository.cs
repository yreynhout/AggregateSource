using System;
using System.IO;
using System.Linq;
using EventStore.ClientAPI;
using ProtoBuf;

namespace AggregateSource.GEventStore {
  public class Repository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : AggregateRootEntity {
    readonly Func<TAggregateRoot> _rootFactory;
    readonly UnitOfWork _unitOfWork;
    readonly EventStoreConnection _connection;

    public Repository(Func<TAggregateRoot> rootFactory, UnitOfWork unitOfWork, EventStoreConnection connection) {
      if (rootFactory == null) throw new ArgumentNullException("rootFactory");
      if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
      if (connection == null) throw new ArgumentNullException("connection");
      _rootFactory = rootFactory;
      _unitOfWork = unitOfWork;
      _connection = connection;
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
      const int sliceSize = 500;
      var slice = _connection.ReadStreamEventsForward(stream, 1, sliceSize, false);
      if (slice.Status == SliceReadStatus.StreamDeleted || slice.Status == SliceReadStatus.StreamNotFound) {
        root = null;
        return false;
      }
      root = _rootFactory();
      root.Initialize(
          slice.Events.Select(
            resolved => DeserializeEvent(resolved.OriginalEvent.EventType, resolved.OriginalEvent.Data)));
      while (!slice.IsEndOfStream) {
        slice = _connection.ReadStreamEventsForward(stream, slice.NextEventNumber, sliceSize, false);
        root.Initialize(
          slice.Events.Select(
            resolved => DeserializeEvent(resolved.OriginalEvent.EventType, resolved.OriginalEvent.Data))); 
      }
      aggregate = new Aggregate(id, slice.LastEventNumber, root);
      _unitOfWork.Attach(aggregate);
      return true;
    }

    static object DeserializeEvent(string eventType, byte[] data) {
      var type = Type.GetType(eventType, true);
      using (var stream = new MemoryStream(data)) {
        return Serializer.NonGeneric.Deserialize(type, stream);
      }
    }

    public void Add(Guid id, TAggregateRoot root) {
      _unitOfWork.Attach(new Aggregate(id, Aggregate.InitialVersion, root));
    }
  }
}
