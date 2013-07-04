namespace AggregateSource.GEventStore.Snapshots.Framework {
  public static class SnapshotStoreReadConfigurationFactory {
    public static SnapshotStoreReadConfiguration Create() {
      return new SnapshotStoreReadConfiguration(new SnapshotStreamNameResolver(), new SnapshotDeserializer());
    }

    public static SnapshotStoreReadConfiguration CreateWithResolver(IStreamNameResolver resolver) {
      return new SnapshotStoreReadConfiguration(resolver, new SnapshotDeserializer());
    }
  }
}