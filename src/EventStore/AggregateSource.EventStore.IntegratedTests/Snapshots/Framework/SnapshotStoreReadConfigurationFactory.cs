using AggregateSource.EventStore.Resolvers;
using EventStore.ClientAPI.SystemData;

namespace AggregateSource.EventStore.Snapshots.Framework {
  public static class SnapshotStoreReadConfigurationFactory {
    public static SnapshotStoreReadConfiguration Create() {
      return new SnapshotStoreReadConfiguration(new SnapshotDeserializer(), new SnapshotStreamNameResolver(), new FixedStreamUserCredentialsResolver(new UserCredentials("admin", "changeit")));
    }

    public static SnapshotStoreReadConfiguration CreateWithResolver(IStreamNameResolver resolver) {
      return new SnapshotStoreReadConfiguration(new SnapshotDeserializer(), resolver, new FixedStreamUserCredentialsResolver(new UserCredentials("admin", "changeit")));
    }
  }
}