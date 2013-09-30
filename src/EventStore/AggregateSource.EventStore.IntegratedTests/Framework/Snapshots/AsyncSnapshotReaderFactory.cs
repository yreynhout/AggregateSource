using AggregateSource.EventStore.Snapshots;
using EventStore.ClientAPI;

namespace AggregateSource.EventStore.Framework.Snapshots
{
    public static class AsyncSnapshotReaderFactory
    {
        public static AsyncSnapshotReader Create()
        {
            return Create(EmbeddedEventStore.Connection, SnapshotReaderConfigurationFactory.Create());
        }

        public static AsyncSnapshotReader CreateWithConfiguration(SnapshotReaderConfiguration configuration)
        {
            return Create(EmbeddedEventStore.Connection, configuration);
        }

        public static AsyncSnapshotReader CreateWithConnection(IEventStoreConnection connection)
        {
            return Create(connection, SnapshotReaderConfigurationFactory.Create());
        }

        public static AsyncSnapshotReader Create(IEventStoreConnection connection,
                                                 SnapshotReaderConfiguration configuration)
        {
            return new AsyncSnapshotReader(connection, configuration);
        }
    }
}