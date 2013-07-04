using System;
using AggregateSource.GEventStore.Snapshots.Framework;

namespace AggregateSource.GEventStore.Snapshots {
  public class SnapshotableAggregateRootEntityStub : AggregateRootEntity, ISnapshotable {
    public static readonly Func<SnapshotableAggregateRootEntityStub> Factory = () => new SnapshotableAggregateRootEntityStub();

    public object RestoredSnapshot { get; private set; }

    public void RestoreSnapshot(object state) {
      RestoredSnapshot = state;
    }

    public object TakeSnapshot() {
      return new SnapshotState();
    }
  }
}
