using System;
using System.IO;
using System.Linq;
using EventStore.ClientAPI;
using ProtoBuf;

namespace AggregateSource.EventStorage {
  //This class is still a mess ... once we have UoW scoping things will fall into place.
  public class EventStoreAwareHandler : IHandle<RegisterBirthOfDogCommand>, IHandle<RegisterThatTheDogGotAShot> {
    readonly EventStoreConnection _connection;

    public EventStoreAwareHandler(EventStoreConnection connection) {
      if (connection == null) throw new ArgumentNullException("connection");
      _connection = connection;
    }

    public void Handle(RegisterBirthOfDogCommand message) {
      var unitOfWork = new UnitOfWork();
      var handler = CreateHandler(unitOfWork);
      handler.Handle(message);
      StoreChanges(unitOfWork);
    }

    public void Handle(RegisterThatTheDogGotAShot message) {
      var unitOfWork = new UnitOfWork();
      var handler = CreateHandler(unitOfWork);
      handler.Handle(message);
      StoreChanges(unitOfWork);
    }

    DogApplicationServices CreateHandler(UnitOfWork unitOfWork) {
      var dogRepository = new EventStoreRepository<Dog>(Dog.Factory, _connection, unitOfWork);
      return new DogApplicationServices(dogRepository);
    }

    void StoreChanges(UnitOfWork unitOfWork) {
      if (unitOfWork.HasChanges()) {
        var aggregate = unitOfWork.GetChanges().OfType<EventStoreAggregate>().Single();
        var streamName = GetStreamName(aggregate.Id);
        if (aggregate.ExpectedVersion == EventStoreAggregate.InitialVersion) {
          _connection.CreateStream(streamName, aggregate.Id, false, new byte[0]);
        }
        _connection.AppendToStream(
          streamName,
          aggregate.ExpectedVersion,
          aggregate.Root.GetChanges().Select(change =>
                                             new EventData(Guid.NewGuid(), change.GetType().AssemblyQualifiedName, false,
                                                           SerializeEvent(change), new byte[0])));
      }
    }

    static string GetStreamName(Guid id) {
      return string.Format("DOG{0}", id.ToString().Replace("-", ""));
    }

    static byte[] SerializeEvent(object @event) {
      using (var stream = new MemoryStream()) {
        Serializer.NonGeneric.Serialize(stream, @event);
        return stream.ToArray();
      }
    }
  }
}
