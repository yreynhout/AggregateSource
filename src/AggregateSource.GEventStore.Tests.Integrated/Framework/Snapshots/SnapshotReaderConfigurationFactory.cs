using AggregateSource.GEventStore.Snapshots;

namespace AggregateSource.GEventStore.Framework.Snapshots {
  public static class SnapshotReaderConfigurationFactory {
    public static SnapshotReaderConfiguration Create() {
      return new SnapshotReaderConfiguration(new SnapshotStreamNameResolver(), new SnapshotDeserializer());
    }

    public static SnapshotReaderConfiguration CreateWithResolver(IStreamNameResolver resolver) {
      return new SnapshotReaderConfiguration(resolver, new SnapshotDeserializer());
    }
  }
}