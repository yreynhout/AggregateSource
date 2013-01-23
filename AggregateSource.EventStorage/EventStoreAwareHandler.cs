using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EventStore.ClientAPI;
using ProtoBuf;

namespace AggregateSource.EventStorage {
  //This class is a mess ... once we have UoW scoping things will fall into place.
  public class EventStoreAwareHandler : IHandle<RegisterBirthOfDogCommand>, IHandle<RegisterThatTheDogGotAShot> {
    readonly EventStoreConnection _connection;

    public EventStoreAwareHandler(EventStoreConnection connection) {
      if (connection == null) throw new ArgumentNullException("connection");
      _connection = connection;
    }

    public void Handle(RegisterBirthOfDogCommand message) {
      var unitOfWork = new UnitOfWork();
      var dogRepository = new Repository<Dog>(
        id => {
          var dog = Dog.Factory();
          var slice = ReadSliceFromStore(id);
          dog.Initialize(ReadStreamFromStore(slice));
          return new Aggregate(id, slice.LastEventNumber, dog);
        }, unitOfWork);
      var handler = new DogApplicationServices(dogRepository);

      handler.Handle(message);

      if (unitOfWork.HasChanges()) {
        var aggregate = unitOfWork.GetChanges().Single();
        var streamName = GetStreamName(aggregate.Id);
        if (aggregate.Version == Aggregate.InitialVersion) {
          _connection.CreateStream(streamName, aggregate.Id, true, new byte[0]);
        }
        _connection.AppendToStream(
          streamName, 
          aggregate.Version, 
          aggregate.Root.GetChanges().Select(change => 
            new EventData(Guid.NewGuid(), change.GetType().AssemblyQualifiedName, false, SerializeEvent(change), new byte[0])));
      }
    }

    StreamEventsSlice ReadSliceFromStore(Guid id) {
      return _connection.ReadStreamEventsForward(GetStreamName(id), 0, Int32.MaxValue, false);
    }

    IEnumerable<object> ReadStreamFromStore(StreamEventsSlice slice) {
      return slice.Events.Skip(1).Select(resolvedEvent => DeserializeEvent(resolvedEvent.Event.EventType, resolvedEvent.Event.Data));
    }

    static string GetStreamName(Guid id) {
      return string.Format("DOG{0}", id.ToString().Replace("-", ""));
    }

    static object DeserializeEvent(string typeName, byte[] data) {
      var eventType = Type.GetType(typeName, true);
      using (var stream = new MemoryStream(data)) {
        return Serializer.NonGeneric.Deserialize(eventType, stream);
      }
    }

    static byte[] SerializeEvent(object @event) {
      using (var stream = new MemoryStream()) {
        Serializer.NonGeneric.Serialize(stream, @event);
        return stream.ToArray();
      }
    }

    public void Handle(RegisterThatTheDogGotAShot message) {
      var unitOfWork = new UnitOfWork();
      var dogRepository = new Repository<Dog>(
        id => {
          var dog = Dog.Factory();
          var slice = ReadSliceFromStore(id);
          dog.Initialize(ReadStreamFromStore(slice));
          return new Aggregate(id, slice.LastEventNumber, dog);
        }, unitOfWork);
      var handler = new DogApplicationServices(dogRepository);

      handler.Handle(message);

      if (unitOfWork.HasChanges()) {
        var aggregate = unitOfWork.GetChanges().Single();
        var streamName = GetStreamName(aggregate.Id);
        if (aggregate.Version == Aggregate.InitialVersion) {
          _connection.CreateStream(streamName, aggregate.Id, false, new byte[0]);
        }
        _connection.AppendToStream(
          streamName,
          aggregate.Version,
          aggregate.Root.GetChanges().Select(change =>
            new EventData(Guid.NewGuid(), change.GetType().AssemblyQualifiedName, false, SerializeEvent(change), new byte[0])));
      }
    }
  }
}
