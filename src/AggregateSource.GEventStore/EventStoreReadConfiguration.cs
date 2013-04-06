using System;

namespace AggregateSource.GEventStore {
  public class EventStoreReadConfiguration {
    public static readonly EventStoreReadConfiguration Default = 
      new EventStoreReadConfiguration(
        new SliceSize(500), 
        new ResolvedEventDeserializer());

    readonly SliceSize _sliceSize;
    readonly IResolvedEventDeserializer _deserializer;

    public EventStoreReadConfiguration(SliceSize sliceSize, IResolvedEventDeserializer deserializer) {
      if (deserializer == null) throw new ArgumentNullException("deserializer");
      _sliceSize = sliceSize;
      _deserializer = deserializer;
    }

    public SliceSize SliceSize {
      get { return _sliceSize; }
    }

    public IResolvedEventDeserializer Deserializer {
      get { return _deserializer; }
    }
  }
}
