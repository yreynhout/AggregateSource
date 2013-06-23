using AggregateSource.GEventStore.Resolvers;

namespace AggregateSource.GEventStore.Framework {
  public static class EventStoreReadConfigurationFactory {
    public static EventStoreReadConfiguration NewInstance() {
      return new EventStoreReadConfiguration(new SliceSize(1), new ResolvedEventDeserializer(), new PassThroughStreamNameResolver());
    }

    public static EventStoreReadConfiguration NewInstanceWithStreamNameResolver(IStreamNameResolver streamNameResolver) {
      return new EventStoreReadConfiguration(new SliceSize(1), new ResolvedEventDeserializer(), streamNameResolver);
    }
  }
}