using EventStore.ClientAPI;

namespace AggregateSource.EventStore {
  public interface IResolvedEventDeserializer {
    object Deserialize(ResolvedEvent resolvedEvent);
  }
}