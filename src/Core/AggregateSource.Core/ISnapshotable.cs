namespace AggregateSource
{
    /// <summary>
    /// Represents the snapshotting operations on an aggregate root entity.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Snapshotable")]
    public interface ISnapshotable
    {
        /// <summary>
        /// Restores a snapshot using the specified <paramref name="state"/> object.
        /// </summary>
        /// <param name="state">The state object to restore the snapshot from.</param>
        void RestoreSnapshot(object state);

        /// <summary>
        /// Takes a snapshot of the aggregate root entity.
        /// </summary>
        /// <returns>The state object that represents the snapshot.</returns>
        object TakeSnapshot();
    }
}