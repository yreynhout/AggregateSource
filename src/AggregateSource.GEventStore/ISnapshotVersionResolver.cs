using System;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore {
  /// <summary>
  /// Provides a way to resolve the version of the stream the snapshot was taken at.
  /// </summary>
  public interface ISnapshotVersionResolver {
    /// <summary>
    /// Resolves the version the snapshot was taken at from the resolved event the snapshot was deserialized from.
    /// </summary>
    /// <param name="resolvedEvent">The resolved event.</param>
    /// <returns>The version the snapshot was taken at.</returns>
    Int32 Resolve(ResolvedEvent resolvedEvent);
  }
}
