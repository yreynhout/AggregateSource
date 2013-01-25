using System;
using System.IO;
using System.Linq;
using AggregateSource.Ambient;
using EventStore.ClientAPI;
using ProtoBuf;

namespace AggregateSource.EventStorage {
  public class EventStoreAwareHandler<TMessage> : IHandle<TMessage> {
    readonly EventStoreConnection _connection;
    readonly IHandle<TMessage> _handler;

    public EventStoreAwareHandler(EventStoreConnection connection, IHandle<TMessage> handler) {
      if (connection == null) throw new ArgumentNullException("connection");
      if (handler == null) throw new ArgumentNullException("handler");
      _connection = connection;
      _handler = handler;
    }

    public void Handle(TMessage message) {
      using (var scope = new UnitOfWorkScope(new UnitOfWork())) {
        
        _handler.Handle(message);

        StoreChangesIfAny(scope.UnitOfWork);
      }
    }

    void StoreChangesIfAny(UnitOfWork unitOfWork) {
      const int sliceEventCount = 500;
      if (unitOfWork.HasChanges()) {
        var aggregate = unitOfWork.GetChanges().OfType<EventStoreAggregate>().Single();
        var eventIndex = 0;
        var slices = from eventData in
                       aggregate.Root.
                                 GetChanges().
                                 Select(change => new EventData(
                                                    Guid.NewGuid(),
                                                    change.GetType().AssemblyQualifiedName,
                                                    false,
                                                    SerializeEvent(change),
                                                    new byte[0]))
                     group eventData by eventIndex++%sliceEventCount
                     into slice
                     select slice.AsEnumerable();
        using (var transaction = _connection.StartTransaction(aggregate.Stream, aggregate.ExpectedVersion)) {
          foreach (var slice in slices) {
            transaction.Write(slice);
          }
          transaction.Commit();
        }
      }
    }

    static byte[] SerializeEvent(object @event) {
      using (var stream = new MemoryStream()) {
        Serializer.NonGeneric.Serialize(stream, @event);
        return stream.ToArray();
      }
    }
  }
}
