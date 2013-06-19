using System.IO;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore.Resolvers {
  /// <summary>
  /// Snapshot version resolver that pulls the version the snapshot was taken at from the raw meta data bytes.
  /// </summary>
  public class MetadataSnapshotVersionResolver : ISnapshotVersionResolver {
    /// <summary>
    /// Resolves the version the snapshot was taken at from the resolved event the snapshot was deserialized from.
    /// </summary>
    /// <param name="resolvedEvent">The resolved event.</param>
    /// <returns>
    /// The version the snapshot was taken at.
    /// </returns>
    /// <remarks>The assumption is that the version is stored in the metadata as an integer converted to bytes.</remarks>
    public int Resolve(ResolvedEvent resolvedEvent) {
      using (var stream = new MemoryStream(resolvedEvent.Event.Metadata))
      using (var reader = new BinaryReader(stream)) {
        return reader.ReadInt32();
      }
    }
  }
}
