using System;
using System.IO;
using System.Linq;
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
      if (unitOfWork.HasChanges()) {
        var aggregate = unitOfWork.GetChanges().OfType<EventStoreAggregate>().Single();
        if (aggregate.ExpectedVersion == EventStoreAggregate.InitialVersion) {
          _connection.CreateStream(aggregate.Stream, aggregate.Id, false, new byte[0]);
        }
        _connection.AppendToStream(
          aggregate.Stream,
          aggregate.ExpectedVersion,
          aggregate.Root.GetChanges().
                    Select(change => new EventData(
                                       Guid.NewGuid(),
                                       change.GetType().AssemblyQualifiedName,
                                       false,
                                       SerializeEvent(change),
                                       new byte[0])));
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
