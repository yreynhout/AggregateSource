using System;
using System.Collections.Generic;
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
      var name = new AggregateBasedStreamName(id, typeof (TAggregateRoot));
      var slice = ReadSliceFromStoreByName(name);
      if (slice.Status == SliceReadStatus.StreamNotFound) {
        aggregate = null;
        return false;
      }
      var root = _factory();
      root.Initialize(StreamEventsIntoAggregateRootFromSlice(slice));
      aggregate = new EventStoreAggregate(
        id, 
        root, 
        slice.LastEventNumber,
        new AggregateBasedStreamName(id, typeof (TAggregateRoot)));
      return true;
    }

    protected override Aggregate CreateAggregate(Guid id, TAggregateRoot root) {
      return new EventStoreAggregate(
        id, 
        root, 
        EventStoreAggregate.InitialVersion,
        new AggregateBasedStreamName(id, typeof(TAggregateRoot)));
    }

    StreamEventsSlice ReadSliceFromStoreByName(AggregateBasedStreamName stream) {
      return _connection.ReadStreamEventsForward(stream, 0, Int32.MaxValue, false);
    }

    IEnumerable<object> StreamEventsIntoAggregateRootFromSlice(StreamEventsSlice slice) {
      return slice.Events.
                   Skip(1).
                   Select(
                     resolvedEvent =>
                     DeserializeEvent(resolvedEvent.Event.EventType, resolvedEvent.Event.Data));
    }

    static object DeserializeEvent(string typeName, byte[] data) {
      var eventType = Type.GetType(typeName, true);
      using (var stream = new MemoryStream(data)) {
        return Serializer.NonGeneric.Deserialize(eventType, stream);
      }
    }
  }
}