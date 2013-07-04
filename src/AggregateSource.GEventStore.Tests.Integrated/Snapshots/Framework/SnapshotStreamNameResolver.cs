namespace AggregateSource.GEventStore.Snapshots.Framework {
  public class SnapshotStreamNameResolver : IStreamNameResolver {
    public string Resolve(string identifier) {
      return identifier + "-snapshot";
    }
  }
}