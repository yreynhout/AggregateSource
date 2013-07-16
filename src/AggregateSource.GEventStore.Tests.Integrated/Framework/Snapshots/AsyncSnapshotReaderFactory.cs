using AggregateSource.GEventStore.Snapshots;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore.Framework.Snapshots {
  public static class AsyncSnapshotReaderFactory {
    public static AsyncSnapshotReader Create() {
      return Create(EmbeddedEventStore.Instance.Connection, SnapshotReaderConfigurationFactory.Create());
    }

    public static AsyncSnapshotReader CreateWithConfiguration(SnapshotReaderConfiguration configuration) {
      return Create(EmbeddedEventStore.Instance.Connection, configuration);
    }

    public static AsyncSnapshotReader CreateWithConnection(EventStoreConnection connection) {
      return Create(connection, SnapshotReaderConfigurationFactory.Create());
    }

    public static AsyncSnapshotReader Create(EventStoreConnection connection, SnapshotReaderConfiguration configuration) {
      return new AsyncSnapshotReader(connection, configuration);
    }
  }
}