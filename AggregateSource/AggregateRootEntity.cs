using System;
using System.Collections.Generic;

namespace AggregateSource {
  public abstract class AggregateRootEntity {
    readonly List<object> _changes;
    readonly Dictionary<Type, Action<object>> _handlers;

    protected AggregateRootEntity() {
      _handlers = new Dictionary<Type, Action<object>>();
      _changes = new List<object>();
    }

    protected void Register<TEvent>(Action<TEvent> handler) {
      if (handler == null) throw new ArgumentNullException("handler");
      _handlers.Add(typeof (TEvent), @event => handler((TEvent) @event));
    }

    public void Initialize(IEnumerable<object> events) {
      if (events == null) throw new ArgumentNullException("events");
      ClearChanges();
      foreach (var @event in events) {
        Play(@event);
      }
    }

    protected void Apply(object @event) {
      Play(@event);
      Record(@event);
    }

    void Play(object @event) {
      Action<object> handler;
      if (_handlers.TryGetValue(@event.GetType(), out handler)) {
        handler(@event);
      }
    }

    void Record(object @event) {
      _changes.Add(@event);
    }

    public bool HasChanges() {
      return _changes.Count != 0;
    }

    public IEnumerable<object> GetChanges() {
      return _changes.ToArray();
    }

    public void ClearChanges() {
      _changes.Clear();
    }
  }
}
