using EventStore.ClientAPI;

namespace AggregateSource.EventStore.Snapshots
{
    /// <summary>
    /// Represents a snapshot deserializer.
    /// </summary>
    public interface ISnapshotDeserializer
    {
        /// <summary>
        /// Deserializes a resolved event into an snapshot consumable by the aggregate root entity.
        /// </summary>
        /// <param name="resolvedEvent">The resolved event to deserialize.</param>
        /// <returns>The deserialized snapshot.</returns>
        Snapshot Deserialize(ResolvedEvent resolvedEvent);
    }
}