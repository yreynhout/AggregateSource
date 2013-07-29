namespace AggregateSource {
  public interface IStaticEventRouter {
    void Route(object instance, object @event);
  }
}