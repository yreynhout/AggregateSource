using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore.Snapshots {
  public class AsyncSnapshotReader : IAsyncSnapshotReader {
    readonly EventStoreConnection _connection;
    readonly SnapshotStoreReadConfiguration _configuration;

    public AsyncSnapshotReader(EventStoreConnection connection, SnapshotStoreReadConfiguration configuration) {
      if (connection == null) throw new ArgumentNullException("connection");
      if (configuration == null) throw new ArgumentNullException("configuration");
      _connection = connection;
      _configuration = configuration;
    }

    public async Task<Optional<Snapshot>> ReadOptionalAsync(string identifier) {
      var streamName = _configuration.Resolver.Resolve(identifier);
      var slice = await _connection.ReadStreamEventsBackwardAsync(streamName, StreamPosition.End, 1, false);
      if (slice.Status == SliceReadStatus.StreamDeleted || slice.Status == SliceReadStatus.StreamNotFound || slice.Events.Length == 0) {
        return Optional<Snapshot>.Empty;
      }
      return new Optional<Snapshot>(_configuration.Deserializer.Deserialize(slice.Events[0]));
    }
  }
}