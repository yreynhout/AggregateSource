namespace AggregateSource.EventStore.Snapshots
{
    /// <summary>
    /// Represents the behavior that reads a <see cref="Snapshot"/> from the underlying storage.
    /// </summary>
    public interface ISnapshotReader
    {
        /// <summary>
        /// Reads a snapshot from the underlying storage if one is present.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <returns>A <see cref="Snapshot"/> if found, otherwise <c>empty</c>.</returns>
        Optional<Snapshot> ReadOptional(string identifier);
    }
}