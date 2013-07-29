using System;
using System.Collections.Generic;

namespace AggregateSource {
  public class InstanceEventRouter : IConfigureInstanceEventRouter {
    readonly Dictionary<Type, Action<object>> _handlers;

    public InstanceEventRouter() {
      _handlers = new Dictionary<Type, Action<object>>();
    }

    public void AddRoute(Type @event, Action<object> handler) {
      if (@event == null) throw new ArgumentNullException("event");
      if (handler == null) throw new ArgumentNullException("handler");
      _handlers.Add(@event, handler);
    }

    public void AddRoute<TEvent>(Action<TEvent> handler) {
      if (handler == null) throw new ArgumentNullException("handler");
      _handlers.Add(typeof(TEvent), @event => handler((TEvent) @event));
    }

    public void Route(object @event) {
      Action<object> handler;
      if (_handlers.TryGetValue(@event.GetType(), out handler)) {
        handler(@event);
      }
    }
  }
}