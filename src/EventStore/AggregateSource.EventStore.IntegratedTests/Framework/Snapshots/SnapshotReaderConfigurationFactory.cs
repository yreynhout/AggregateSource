using AggregateSource.EventStore.Resolvers;
using AggregateSource.EventStore.Snapshots;
using EventStore.ClientAPI.SystemData;

namespace AggregateSource.EventStore.Framework.Snapshots
{
    public static class SnapshotReaderConfigurationFactory
    {
        public static SnapshotReaderConfiguration Create()
        {
            return new SnapshotReaderConfiguration(new SnapshotDeserializer(), new SnapshotStreamNameResolver(),
                                                   new FixedStreamUserCredentialsResolver(new UserCredentials("admin",
                                                                                                              "changeit")));
        }

        public static SnapshotReaderConfiguration CreateWithResolver(IStreamNameResolver resolver)
        {
            return new SnapshotReaderConfiguration(new SnapshotDeserializer(), resolver,
                                                   new FixedStreamUserCredentialsResolver(new UserCredentials("admin",
                                                                                                              "changeit")));
        }
    }
}