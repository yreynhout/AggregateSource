using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace AggregateSource.Reactive {
  public class ObservableAggregateRootEntity : IAggregateInitializer, IObservable<object>, IDisposable {
    readonly ReplaySubject<object> _recorder;
    readonly Dictionary<Type, Action<object>> _handlers;
    bool _changed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAggregateRootEntity"/> class.
    /// </summary>
    protected ObservableAggregateRootEntity() {
      _handlers = new Dictionary<Type, Action<object>>();
      _recorder = new ReplaySubject<object>(Int32.MaxValue, TimeSpan.MaxValue);
      _changed = false;
    }

    /// <summary>
    /// Registers the specified handler to be invoked when the specified event is applied.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to register the handler for.</typeparam>
    /// <param name="handler">The handler.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="handler"/> is null.</exception>
    protected void Register<TEvent>(Action<TEvent> handler) {
      if (handler == null) throw new ArgumentNullException("handler");
      _handlers.Add(typeof (TEvent), @event => handler((TEvent) @event));
    }

    /// <summary>
    /// Initializes this instance using the specified events.
    /// </summary>
    /// <param name="events">The events to initialize with.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="events"/> are null.</exception>
    public void Initialize(IEnumerable<object> events) {
      if (events == null) throw new ArgumentNullException("events");
      if (HasChanges()) throw new InvalidOperationException("Initialize cannot be called on an instance with changes.");
      foreach (var @event in events) {
        Play(@event);
      }
    }

    /// <summary>
    /// Applies the specified event to this instance and invokes the associated state handler.
    /// </summary>
    /// <param name="event">The event to apply.</param>
    protected void Apply(object @event) {
      if (@event == null) throw new ArgumentNullException("event");
      BeforeApply(@event);
      Play(@event);
      Record(@event);
      AfterApply(@event);
    }

    /// <summary>
    /// Called before an event is applied, exposed as a point of interception.
    /// </summary>
    /// <param name="event">The event that will be applied.</param>
    protected virtual void BeforeApply(object @event) { }

    /// <summary>
    /// Called after an event has been applied, exposed as a point of interception.
    /// </summary>
    /// <param name="event">The event that has been applied.</param>
    protected virtual void AfterApply(object @event) { }

    void Play(object @event) {
      Action<object> handler;
      if (_handlers.TryGetValue(@event.GetType(), out handler)) {
        handler(@event);
      }
    }

    void Record(object @event) {
      _changed = true;
      _recorder.OnNext(@event);
    }

    bool HasChanges() {
      return _changed;
    }

    public IDisposable Subscribe(IObserver<object> observer) {
      return _recorder.Subscribe(observer);
    }

    public void Dispose() {
      _recorder.Dispose();
    }
  }
}
