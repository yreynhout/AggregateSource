using System;

namespace AggregateSource.GEventStore {
  /// <summary>
  /// Represents configuration settings used during reading from the event store.
  /// </summary>
  public class EventStoreReadConfiguration {
    readonly SliceSize _sliceSize;
    readonly IResolvedEventDeserializer _resolvedEventDeserializer;
    readonly IStreamNameResolver _streamNameResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreReadConfiguration"/> class.
    /// </summary>
    /// <param name="sliceSize">Size of the slice to read.</param>
    /// <param name="resolvedEventDeserializer">The resolved event deserializer to use.</param>
    /// <param name="streamNameResolver">The stream name resolver to use.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="resolvedEventDeserializer"/> is null.</exception>
    /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="streamNameResolver"/> is null.</exception>
    public EventStoreReadConfiguration(SliceSize sliceSize, IResolvedEventDeserializer resolvedEventDeserializer, IStreamNameResolver streamNameResolver) {
      if (resolvedEventDeserializer == null) throw new ArgumentNullException("resolvedEventDeserializer");
      if (streamNameResolver == null) throw new ArgumentNullException("streamNameResolver");
      _sliceSize = sliceSize;
      _resolvedEventDeserializer = resolvedEventDeserializer;
      _streamNameResolver = streamNameResolver;
    }

    /// <summary>
    /// Gets the size of the slice to read.
    /// </summary>
    /// <value>
    /// The size of the slice to read.
    /// </value>
    public SliceSize SliceSize {
      get { return _sliceSize; }
    }

    /// <summary>
    /// Gets the resolved event deserializer.
    /// </summary>
    /// <value>
    /// The resolved event deserializer.
    /// </value>
    public IResolvedEventDeserializer ResolvedEventDeserializer {
      get { return _resolvedEventDeserializer; }
    }

    /// <summary>
    /// Gets the stream name resolver.
    /// </summary>
    /// <value>The stream name resolver.</value>
    public IStreamNameResolver StreamNameResolver {
      get { return _streamNameResolver; }
    }
  }
}
