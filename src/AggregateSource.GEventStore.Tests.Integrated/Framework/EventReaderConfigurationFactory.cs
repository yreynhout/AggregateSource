using AggregateSource.GEventStore.Resolvers;

namespace AggregateSource.GEventStore.Framework {
  public static class EventReaderConfigurationFactory {
    public static EventReaderConfiguration Create() {
      return new EventReaderConfiguration(new SliceSize(1), new EventDeserializer(), new PassThroughStreamNameResolver());
    }

    public static EventReaderConfiguration CreateWithResolver(IStreamNameResolver resolver) {
      return new EventReaderConfiguration(new SliceSize(1), new EventDeserializer(), resolver);
    }
  }
}