using System;

namespace AggregateSource.GEventStore.Snapshots {
  /// <summary>
  /// Represents configuration settings used during reading from the snapshot store.
  /// </summary>
  public class SnapshotReaderConfiguration {
    readonly IStreamNameResolver _resolver;
    readonly ISnapshotDeserializer _deserializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="SnapshotReaderConfiguration"/> class.
    /// </summary>
    /// <param name="resolver">The snapshot stream name resolver.</param>
    /// <param name="deserializer">The snapshot deserializer.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="resolver"/> or <paramref name="deserializer"/> are <c>null</c>.</exception>
    public SnapshotReaderConfiguration(IStreamNameResolver resolver, ISnapshotDeserializer deserializer) {
      if (resolver == null) throw new ArgumentNullException("resolver");
      if (deserializer == null) throw new ArgumentNullException("deserializer");
      _resolver = resolver;
      _deserializer = deserializer;
    }

    /// <summary>
    /// Gets the snapshot stream name resolver.
    /// </summary>
    /// <value>
    /// The resolver.
    /// </value>
    public IStreamNameResolver Resolver {
      get { return _resolver; }
    }

    /// <summary>
    /// Gets the snapshot deserializer.
    /// </summary>
    /// <value>
    /// The deserializer.
    /// </value>
    public ISnapshotDeserializer Deserializer {
      get { return _deserializer; }
    }
  }
}