using System;

namespace AggregateSource.EventStorage {
  public class EventStoreAggregate : Aggregate {
    public static readonly int InitialVersion = 0;

    readonly int _expectedVersion;

    public EventStoreAggregate(Guid id, int expectedVersion, AggregateRootEntity root) : base(id, root) {
      _expectedVersion = expectedVersion;
    }

    public int ExpectedVersion {
      get { return _expectedVersion; }
    }
  }
}