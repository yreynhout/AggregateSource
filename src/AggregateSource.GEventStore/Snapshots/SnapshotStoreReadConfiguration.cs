using System;

namespace AggregateSource.GEventStore.Snapshots {
  public class SnapshotStoreReadConfiguration {
    readonly IStreamNameResolver _resolver;
    readonly ISnapshotDeserializer _deserializer;

    public SnapshotStoreReadConfiguration(IStreamNameResolver resolver, ISnapshotDeserializer deserializer) {
      if (resolver == null) throw new ArgumentNullException("resolver");
      if (deserializer == null) throw new ArgumentNullException("deserializer");
      _resolver = resolver;
      _deserializer = deserializer;
    }

    public IStreamNameResolver Resolver {
      get { return _resolver; }
    }

    public ISnapshotDeserializer Deserializer {
      get { return _deserializer; }
    }
  }
}