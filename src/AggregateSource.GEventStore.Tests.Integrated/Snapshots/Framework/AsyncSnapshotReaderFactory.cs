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

    public static AsyncSnapshotReader CreateWithConnection(IEventStoreConnection connection) {
      return Create(connection, SnapshotStoreReadConfigurationFactory.Create());
    }

    public static AsyncSnapshotReader Create(IEventStoreConnection connection, SnapshotStoreReadConfiguration configuration) {
      return new AsyncSnapshotReader(connection, configuration);
    }
  }
}