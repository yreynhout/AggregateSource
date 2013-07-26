using AggregateSource.GEventStore.Snapshots;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore.Stubs
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