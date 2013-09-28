using AggregateSource.EventStore.Snapshots;
using EventStore.ClientAPI;

namespace AggregateSource.EventStore.Stubs
{
    public class StubbedSnapshotDeserializer : ISnapshotDeserializer
    {
        public static readonly ISnapshotDeserializer Instance = new StubbedSnapshotDeserializer();

        StubbedSnapshotDeserializer() {}

        public Snapshot Deserialize(ResolvedEvent resolvedEvent)
        {
            return null;
        }
    }
}