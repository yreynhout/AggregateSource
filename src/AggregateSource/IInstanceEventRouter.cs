namespace AggregateSource {
  public interface IInstanceEventRouter {
    void Route(object @event);
  }
}