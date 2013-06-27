namespace AggregateSource.GEventStore.Snapshots {
  /// <summary>
  /// 
  /// </summary>
  public interface ISnapshotReader {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    Optional<Snapshot> ReadOptional(string identifier);
  }
}
