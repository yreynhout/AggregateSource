using System;
using System.IO;
using System.Linq;
using EventStore.ClientAPI;
using ProtoBuf;

namespace AggregateSource.EventStorage {
  public class EventStoreRepository<TAggregateRoot> : Repository<TAggregateRoot> where TAggregateRoot : AggregateRootEntity {
    readonly Func<TAggregateRoot> _factory;
    readonly EventStoreConnection _connection;

    public EventStoreRepository(Func<TAggregateRoot> factory, EventStoreConnection connection, UnitOfWork unitOfWork) 
      : base(unitOfWork) {
      if (factory == null) throw new ArgumentNullException("factory");
      if (connection == null) throw new ArgumentNullException("connection");
      _factory = factory;
      _connection = connection;
    }

    public EventStoreRepository(Func<TAggregateRoot> factory, EventStoreConnection connection) {
      if (factory == null) throw new ArgumentNullException("factory");
      if (connection == null) throw new ArgumentNullException("connection");
      _factory = factory;
      _connection = connection;
    }

    protected override bool TryReadAggregate(Guid id, out Aggregate aggregate) {
      const int sliceEventCount = 500;
      var name = new AggregateBasedStreamName(id, typeof (TAggregateRoot));
      var slice = _connection.ReadStreamEventsForward(name, 0, sliceEventCount, false);
      if (slice.Status == SliceReadStatus.StreamDeleted || slice.Status == SliceReadStatus.StreamNotFound) {
        aggregate = null;
        return false;
      }
      var root = _factory();
      root.Initialize(slice.Events.Skip(1).Select(resolved => DeserializeEvent(resolved.OriginalEvent.EventType, resolved.OriginalEvent.Data)));
      var nextEventNumber = slice.NextEventNumber;
      while (!slice.IsEndOfStream) {
        slice = _connection.ReadStreamEventsForward(name, nextEventNumber, sliceEventCount, false);
        root.Initialize(slice.Events.Select(resolved => DeserializeEvent(resolved.OriginalEvent.EventType, resolved.OriginalEvent.Data)));
        nextEventNumber = slice.NextEventNumber;
      }
      aggregate = new EventStoreAggregate(id, root, slice.LastEventNumber, name);
      return true;
    }

    protected override Aggregate CreateAggregate(Guid id, TAggregateRoot root) {
      return new EventStoreAggregate(
        id, 
        root, 
        ExpectedVersion.NoStream,
        new AggregateBasedStreamName(id, typeof(TAggregateRoot)));
    }

    static object DeserializeEvent(string typeName, byte[] data) {
      var eventType = Type.GetType(typeName, true);
      using (var stream = new MemoryStream(data)) {
        return Serializer.NonGeneric.Deserialize(eventType, stream);
      }
    }
  }
}