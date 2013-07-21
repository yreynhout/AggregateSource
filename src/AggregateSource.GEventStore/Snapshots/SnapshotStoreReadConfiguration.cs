using System;

namespace AggregateSource.GEventStore.Snapshots {
  /// <summary>
  /// Represents configuration settings used during reading from the snapshot store.
  /// </summary>
  public class SnapshotStoreReadConfiguration {
    readonly ISnapshotDeserializer _deserializer;
    readonly IStreamNameResolver _streamNameResolver;
    readonly IStreamUserCredentialsResolver _streamUserCredentialsResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="SnapshotStoreReadConfiguration"/> class.
    /// </summary>
    /// <param name="deserializer">The snapshot deserializer.</param>
    /// <param name="streamNameResolver">The snapshot stream name resolver.</param>
    /// <param name="streamUserCredentialsResolver">The snapshot stream user credentials resolver.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="streamNameResolver"/> or <paramref name="deserializer"/> are <c>null</c>.</exception>
    public SnapshotStoreReadConfiguration(ISnapshotDeserializer deserializer, IStreamNameResolver streamNameResolver, IStreamUserCredentialsResolver streamUserCredentialsResolver) {
      if (streamNameResolver == null) throw new ArgumentNullException("streamNameResolver");
      if (streamUserCredentialsResolver == null) throw new ArgumentNullException("streamUserCredentialsResolver");
      if (deserializer == null) throw new ArgumentNullException("deserializer");
      _streamNameResolver = streamNameResolver;
      _streamUserCredentialsResolver = streamUserCredentialsResolver;
      _deserializer = deserializer;
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

    /// <summary>
    /// Gets the snapshot stream name resolver.
    /// </summary>
    /// <value>
    /// The resolver.
    /// </value>
    public IStreamNameResolver StreamNameResolver {
      get { return _streamNameResolver; }
    }

    /// <summary>
    /// Gets the snapshot stream user credentials resolver.
    /// </summary>
    /// <value>
    /// The resolver.
    /// </value>
    public IStreamUserCredentialsResolver StreamUserCredentialsResolver {
      get { return _streamUserCredentialsResolver; }
    }
  }
}