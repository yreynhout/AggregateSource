using System;

namespace AggregateSource.GEventStore {
  /// <summary>
  /// Represents configuration settings used during reading from the event store.
  /// </summary>
  public class EventStoreReadConfiguration {
    readonly SliceSize _sliceSize;
    readonly IEventDeserializer _deserializer;
    readonly IStreamNameResolver _resolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreReadConfiguration"/> class.
    /// </summary>
    /// <param name="sliceSize">Size of the slice to read.</param>
    /// <param name="deserializer">The event deserializer to use.</param>
    /// <param name="resolver">The stream name resolver to use.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="deserializer"/> or <paramref name="resolver"/> is <c>null</c>.</exception>
    public EventStoreReadConfiguration(SliceSize sliceSize, IEventDeserializer deserializer, IStreamNameResolver resolver) {
      if (deserializer == null) throw new ArgumentNullException("deserializer");
      if (resolver == null) throw new ArgumentNullException("resolver");
      _sliceSize = sliceSize;
      _deserializer = deserializer;
      _resolver = resolver;
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
    /// Gets the event deserializer.
    /// </summary>
    /// <value>
    /// The event deserializer.
    /// </value>
    public IEventDeserializer Deserializer {
      get { return _deserializer; }
    }

    /// <summary>
    /// Gets the stream name resolver.
    /// </summary>
    /// <value>The stream name resolver.</value>
    public IStreamNameResolver Resolver {
      get { return _resolver; }
    }
  }
}
