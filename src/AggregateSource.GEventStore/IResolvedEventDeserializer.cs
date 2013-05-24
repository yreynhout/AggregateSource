using EventStore.ClientAPI;

namespace AggregateSource.GEventStore {
  /// <summary>
  /// Represent a resolved event deserializer.
  /// </summary>
  public interface IResolvedEventDeserializer {
    /// <summary>
    /// Deserializes a resolved event into an event consumable by the aggregate root entity.
    /// </summary>
    /// <param name="resolvedEvent">The resolved event.</param>
    /// <returns>The deserialized event.</returns>
    object Deserialize(ResolvedEvent resolvedEvent);
  }
}