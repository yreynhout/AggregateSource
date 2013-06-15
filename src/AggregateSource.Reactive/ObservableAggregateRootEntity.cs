using System;
using System.Reactive.Subjects;

namespace AggregateSource.Reactive {
  public class ObservableAggregateRootEntity : AggregateRootEntity, IObservable<object>, IDisposable {
    readonly ReplaySubject<object> _recorder;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableAggregateRootEntity"/> class.
    /// </summary>
    protected ObservableAggregateRootEntity() {
      _recorder = new ReplaySubject<object>(Int32.MaxValue, TimeSpan.MaxValue);
    }

    protected override void Record(object @event) {
      base.Record(@event);
      _recorder.OnNext(@event);
    }

    public IDisposable Subscribe(IObserver<object> observer) {
      return _recorder.Subscribe(observer);
    }

    public void Dispose() {
      _recorder.Dispose();
    }
  }
}
