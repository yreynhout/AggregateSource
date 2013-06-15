using System;
using System.Collections.Generic;

namespace AggregateSource.Reactive {
  public class ObservableAggregateRootEntity : AggregateRootEntity, IObservable<object>, IDisposable {
    readonly List<IObserver<object>> _observers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAggregateRootEntity"/> class.
    /// </summary>
    protected ObservableAggregateRootEntity() {
      _observers = new List<IObserver<object>>();
    }

    public IDisposable Subscribe(IObserver<object> observer) {
      if (observer == null) throw new ArgumentNullException("observer");
      _observers.Add(observer);
      CatchUp(observer);
      return new Subscription(() => _observers.Remove(observer));
    }

    void CatchUp(IObserver<object> observer) {
      foreach (var change in GetChanges()) {
        observer.OnNext(change);
      }
    }

    public void Dispose() {
      foreach (var observer in _observers) {
        observer.OnCompleted();
      }
    }

    class Subscription : IDisposable {
      bool _disposed;
      readonly Action _disposer;

      public Subscription(Action disposer) {
        _disposer = disposer;
        _disposed = false;
      }

      public void Dispose() {
        if (_disposed) return;
        _disposed = true;
        _disposer();
      }
    }
  }
}
