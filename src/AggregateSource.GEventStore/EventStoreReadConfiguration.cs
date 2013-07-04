using System;

namespace AggregateSource.GEventStore {
  /// <summary>
  /// Represents configuration settings used during reading from the event store.
  /// </summary>
  public class EventStoreReadConfiguration {
    readonly SliceSize _sliceSize;
    readonly IEventDeserializer _eventDeserializer;
    readonly IStreamNameResolver _streamNameResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreReadConfiguration"/> class.
    /// </summary>
    /// <param name="sliceSize">Size of the slice to read.</param>
    /// <param name="eventDeserializer">The resolved event deserializer to use.</param>
    /// <param name="streamNameResolver">The stream name resolver to use.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="eventDeserializer"/> is null.</exception>
    /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="streamNameResolver"/> is null.</exception>
    public EventStoreReadConfiguration(SliceSize sliceSize, IEventDeserializer eventDeserializer, IStreamNameResolver streamNameResolver) {
      if (eventDeserializer == null) throw new ArgumentNullException("eventDeserializer");
      if (streamNameResolver == null) throw new ArgumentNullException("streamNameResolver");
      _sliceSize = sliceSize;
      _eventDeserializer = eventDeserializer;
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
    public IEventDeserializer EventDeserializer {
      get { return _eventDeserializer; }
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
