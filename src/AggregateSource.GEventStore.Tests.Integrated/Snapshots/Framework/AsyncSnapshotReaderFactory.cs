using AggregateSource.GEventStore.Framework;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore.Snapshots.Framework {
  public static class AsyncSnapshotReaderFactory {
    public static AsyncSnapshotReader Create() {
      return Create(EmbeddedEventStore.Instance.Connection, SnapshotStoreReadConfigurationFactory.Create());
    }

    public static AsyncSnapshotReader CreateWithConfiguration(SnapshotStoreReadConfiguration configuration) {
      return Create(EmbeddedEventStore.Instance.Connection, configuration);
    }

    public static AsyncSnapshotReader CreateWithConnection(EventStoreConnection connection) {
      return Create(connection, SnapshotStoreReadConfigurationFactory.Create());
    }

    public static AsyncSnapshotReader Create(EventStoreConnection connection, SnapshotStoreReadConfiguration configuration) {
      return new AsyncSnapshotReader(connection, configuration);
    }
  }
}