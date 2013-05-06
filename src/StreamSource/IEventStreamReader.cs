using System;
using AggregateSource;

namespace StreamSource {
  /// <summary>
  /// Reads the events from an underlying stream provider.
  /// </summary>
  public interface IEventStreamReader {
    /// <summary>
    /// Reads the event associated with the specified stream identifier.
    /// </summary>
    /// <param name="streamId">The stream identifier.</param>
    /// <returns>The found <see cref="EventStream"/>, or empty if not found.</returns>
    Optional<EventStream> Read(string streamId);
  }
}