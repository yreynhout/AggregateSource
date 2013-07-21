using AggregateSource.GEventStore.Resolvers;
using EventStore.ClientAPI.SystemData;

namespace AggregateSource.GEventStore.Framework {
  public static class EventStoreReadConfigurationFactory {
    public static EventStoreReadConfiguration Create() {
      return new EventStoreReadConfiguration(new SliceSize(1), new EventDeserializer(), new PassThroughStreamNameResolver(), new FixedStreamUserCredentialsResolver(new UserCredentials("admin", "changeit")));
    }

    public static EventStoreReadConfiguration CreateWithResolver(IStreamNameResolver resolver) {
      return new EventStoreReadConfiguration(new SliceSize(1), new EventDeserializer(), resolver, new FixedStreamUserCredentialsResolver(new UserCredentials("admin", "changeit")));
    }
  }
}