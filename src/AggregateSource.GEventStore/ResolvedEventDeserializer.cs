using System;
using System.IO;
using EventStore.ClientAPI;
using ProtoBuf;

namespace AggregateSource.GEventStore {
  public class ResolvedEventDeserializer : IResolvedEventDeserializer {
    public object Deserialize(ResolvedEvent resolvedEvent) {
      using (var stream = new MemoryStream(resolvedEvent.OriginalEvent.Data, false)) {
        return Serializer.NonGeneric.Deserialize(
          Type.GetType(resolvedEvent.OriginalEvent.EventType, true), 
          stream);
      }
    }
  }
}