using System;

namespace AggregateSource {
  public interface IConfigureInstanceEventRouter : IInstanceEventRouter {
    void AddRoute(Type @event, Action<object> handler);
    void AddRoute<TEvent>(Action<TEvent> handler);
  }
}