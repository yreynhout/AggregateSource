using EventStore.ClientAPI;

namespace AggregateSource.GEventStore.Snapshots {
  public interface ISnapshotDeserializer {
    Snapshot Deserialize(ResolvedEvent resolvedEvent);
  }
}