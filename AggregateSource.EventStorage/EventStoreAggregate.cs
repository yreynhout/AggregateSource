using System;

namespace AggregateSource.EventStorage {
  public class EventStoreAggregate : Aggregate {
    public static readonly int InitialVersion = 0;

    readonly int _expectedVersion;
    readonly string _stream;

    public EventStoreAggregate(Guid id, AggregateRootEntity root, int expectedVersion, string stream) : base(id, root) {
      if (stream == null) throw new ArgumentNullException("stream");
      _expectedVersion = expectedVersion;
      _stream = stream;
    }

    public int ExpectedVersion {
      get { return _expectedVersion; }
    }

    public string Stream {
      get { return _stream; }
    }
  }
}