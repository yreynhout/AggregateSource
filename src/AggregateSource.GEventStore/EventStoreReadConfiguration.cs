using System;

namespace AggregateSource.GEventStore {
  /// <summary>
  /// Represents configuration settings used during reading from the event store.
  /// </summary>
  public class EventStoreReadConfiguration {
    readonly SliceSize _sliceSize;
    readonly IResolvedEventDeserializer _deserializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreReadConfiguration"/> class.
    /// </summary>
    /// <param name="sliceSize">Size of the slice to read.</param>
    /// <param name="deserializer">The event deserializer to use.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="deserializer"/> is null.</exception>
    public EventStoreReadConfiguration(SliceSize sliceSize, IResolvedEventDeserializer deserializer) {
      if (deserializer == null) throw new ArgumentNullException("deserializer");
      _sliceSize = sliceSize;
      _deserializer = deserializer;
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
    public IResolvedEventDeserializer Deserializer {
      get { return _deserializer; }
    }
  }
}
