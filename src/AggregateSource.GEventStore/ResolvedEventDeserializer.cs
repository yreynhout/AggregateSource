using System;
using System.IO;
using EventStore.ClientAPI;
using ProtoBuf;

namespace AggregateSource.GEventStore {
  /// <summary>
  /// Represent a default, protobuf based resolved event deserializer that gets its event type information from the stream itself.
  /// </summary>
  public class ResolvedEventDeserializer : IResolvedEventDeserializer {
    /// <summary>
    /// Deserializes a resolved event into an event consumable by the aggregate root entity.
    /// </summary>
    /// <param name="resolvedEvent">The resolved event.</param>
    /// <returns>
    /// The deserialized event.
    /// </returns>
    public object Deserialize(ResolvedEvent resolvedEvent) {
      using (var stream = new MemoryStream(resolvedEvent.OriginalEvent.Data, false)) {
        return Serializer.NonGeneric.Deserialize(
          Type.GetType(resolvedEvent.OriginalEvent.EventType, true), 
          stream);
      }
    }
  }
}