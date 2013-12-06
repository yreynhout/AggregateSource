using System.Collections.Generic;
using EventStore.ClientAPI;

namespace AggregateSource.EventStore
{
    /// <summary>
    /// Represent an event deserializer.
    /// </summary>
    public interface IEventDeserializer
    {
        /// <summary>
        /// Deserializes a resolved event into zero, one or more events consumable by the aggregate root entity.
        /// </summary>
        /// <param name="resolvedEvent">The resolved event to deserialize.</param>
        /// <returns>An enumeration of deserialized events.</returns>
        IEnumerable<object> Deserialize(ResolvedEvent resolvedEvent);
    }
}