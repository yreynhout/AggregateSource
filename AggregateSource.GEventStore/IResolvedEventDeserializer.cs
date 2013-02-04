using EventStore.ClientAPI;

namespace AggregateSource.GEventStore {
  public interface IResolvedEventDeserializer {
    object Deserialize(ResolvedEvent resolvedEvent);
  }
}