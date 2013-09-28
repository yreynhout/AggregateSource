using AggregateSource.EventStore.Snapshots;
using EventStore.ClientAPI;

namespace AggregateSource.EventStore.Framework.Snapshots
{
    public static class SnapshotReaderFactory
    {
        public static SnapshotReader Create()
        {
            return Create(EmbeddedEventStore.Connection, SnapshotReaderConfigurationFactory.Create());
        }

        public static SnapshotReader CreateWithConfiguration(SnapshotReaderConfiguration configuration)
        {
            return Create(EmbeddedEventStore.Connection, configuration);
        }

        public static SnapshotReader CreateWithConnection(IEventStoreConnection connection)
        {
            return Create(connection, SnapshotReaderConfigurationFactory.Create());
        }

        public static SnapshotReader Create(IEventStoreConnection connection, SnapshotReaderConfiguration configuration)
        {
            return new SnapshotReader(connection, configuration);
        }
    }
}