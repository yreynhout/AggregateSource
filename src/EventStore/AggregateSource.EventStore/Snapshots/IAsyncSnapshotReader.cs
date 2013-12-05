#if !NET40
using System.Threading.Tasks;

namespace AggregateSource.EventStore.Snapshots
{
    /// <summary>
    /// Represents the async behavior that reads a <see cref="Snapshot"/> from the underlying storage.
    /// </summary>
    public interface IAsyncSnapshotReader
    {
        /// <summary>
        /// Reads a snapshot from the underlying storage if one is present.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <returns>A <see cref="Snapshot"/> if found, otherwise <c>empty</c>.</returns>
        Task<Optional<Snapshot>> ReadOptionalAsync(string identifier);
    }
}
#endif