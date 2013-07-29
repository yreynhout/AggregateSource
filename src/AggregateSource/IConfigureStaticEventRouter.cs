using System;

namespace AggregateSource {
  public interface IConfigureStaticEventRouter : IStaticEventRouter {
    void AddRoute<TInstance, TEvent>(Action<TInstance, TEvent> handler);
    void AddRoute(Type instance, Type @event, Action<object, object> handler);
  }
}