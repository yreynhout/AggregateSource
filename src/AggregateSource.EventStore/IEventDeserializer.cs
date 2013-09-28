using EventStore.ClientAPI;

namespace AggregateSource.EventStore
{
    /// <summary>
    /// Represent an event deserializer.
    /// </summary>
    public interface IEventDeserializer
    {
        /// <summary>
        /// Deserializes a resolved event into an event consumable by the aggregate root entity.
        /// </summary>
        /// <param name="resolvedEvent">The resolved event to deserialize.</param>
        /// <returns>The deserialized event.</returns>
        object Deserialize(ResolvedEvent resolvedEvent);
    }
}