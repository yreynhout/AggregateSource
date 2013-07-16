namespace AggregateSource.GEventStore.Framework.Snapshots {
  public class SnapshotStreamNameResolver : IStreamNameResolver {
    public string Resolve(string identifier) {
      return identifier + "-snapshot";
    }
  }
}