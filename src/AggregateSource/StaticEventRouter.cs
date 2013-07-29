using System;
using System.Collections.Generic;

namespace AggregateSource {
  public class StaticEventRouter : IConfigureStaticEventRouter {
    readonly Dictionary<Type, Dictionary<Type, Action<object, object>>> _index;

    public StaticEventRouter() {
      _index = new Dictionary<Type, Dictionary<Type, Action<object,object>>>();
    }

    public void Route(object instance, object @event) {
      if (instance == null) throw new ArgumentNullException("instance");
      if (@event == null) throw new ArgumentNullException("event");
      Dictionary<Type, Action<object,object>> handlers;
      if (!_index.TryGetValue(instance.GetType(), out handlers)) return;
      Action<object,object> handler;
      if (handlers.TryGetValue(@event.GetType(), out handler)) {
        handler(instance, @event);
      }
    }

    public void AddRoute<TInstance, TEvent>(Action<TInstance, TEvent> handler) {
      if (handler == null) throw new ArgumentNullException("handler");
      Dictionary<Type, Action<object, object>> handlers;
      if (!_index.TryGetValue(typeof (TInstance), out handlers)) {
        handlers = new Dictionary<Type, Action<object, object>>();
      }
      handlers.Add(typeof(TEvent), (instance, @event) => handler((TInstance) instance,(TEvent) @event));
    }

    public void AddRoute(Type instance, Type @event, Action<object, object> handler) {
      if (instance == null) throw new ArgumentNullException("instance");
      if (@event == null) throw new ArgumentNullException("event");
      if (handler == null) throw new ArgumentNullException("handler");
      Dictionary<Type, Action<object, object>> handlers;
      if (!_index.TryGetValue(instance, out handlers)) {
        handlers = new Dictionary<Type, Action<object, object>>();
      }
      handlers.Add(@event, handler);
    }
  }
}