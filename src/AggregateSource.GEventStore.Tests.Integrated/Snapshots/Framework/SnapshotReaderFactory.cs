using AggregateSource.GEventStore.Framework;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore.Snapshots.Framework {
  public static class SnapshotReaderFactory {
    public static SnapshotReader Create() {
      return Create(EmbeddedEventStore.Instance.Connection, SnapshotStoreReadConfigurationFactory.Create());
    }

    public static SnapshotReader CreateWithConfiguration(SnapshotStoreReadConfiguration configuration) {
      return Create(EmbeddedEventStore.Instance.Connection, configuration);
    }

    public static SnapshotReader CreateWithConnection(IEventStoreConnection connection) {
      return Create(connection, SnapshotStoreReadConfigurationFactory.Create());
    }

    public static SnapshotReader Create(IEventStoreConnection connection, SnapshotStoreReadConfiguration configuration) {
      return new SnapshotReader(connection, configuration);
    }
  }
}