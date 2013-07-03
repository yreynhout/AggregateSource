using System;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore.Snapshots {
  public class SnapshotReader : ISnapshotReader {
    readonly EventStoreConnection _connection;
    readonly SnapshotStoreReadConfiguration _configuration;

    public SnapshotReader(EventStoreConnection connection, SnapshotStoreReadConfiguration configuration) {
      if (connection == null) throw new ArgumentNullException("connection");
      if (configuration == null) throw new ArgumentNullException("configuration");
      _connection = connection;
      _configuration = configuration;
    }

    public Optional<Snapshot> ReadOptional(string identifier) {
      if (identifier == null) throw new ArgumentNullException("identifier");
      var streamName = _configuration.Resolver.Resolve(identifier);
      var slice = _connection.ReadStreamEventsBackward(streamName, StreamPosition.End, 1, false);
      if (slice.Status == SliceReadStatus.StreamDeleted || slice.Status == SliceReadStatus.StreamNotFound || slice.Events.Length == 0 || slice.NextEventNumber == -1) {
        return Optional<Snapshot>.Empty;
      }
      return new Optional<Snapshot>(_configuration.Deserializer.Deserialize(slice.Events[0]));
    }
  }
}