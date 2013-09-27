using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AggregateSource.Reactive
{
    /// <summary>
    /// Base class for aggregate root entities that need some basic infrastructure for tracking and publishing state changes.
    /// </summary>
    public class ObservableAggregateRootEntity : IObservableAggregateRootEntity
    {
        readonly EventRecorder _recorder;
        readonly IConfigureInstanceEventRouter _router;

        ImmutableObserverList _observers;
        bool _disposed;
        readonly object _gate = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableAggregateRootEntity"/> class.
        /// </summary>
        protected ObservableAggregateRootEntity()
        {
            _router = new InstanceEventRouter();
            _recorder = new EventRecorder();

            _observers = ImmutableObserverList.Empty;
            _disposed = false;
        }

        /// <summary>
        /// Registers the specified handler to be invoked when the specified event is applied.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to register the handler for.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="handler"/> is null.</exception>
        protected void Register<TEvent>(Action<TEvent> handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            //ThrowIfDisposed();
            _router.ConfigureRoute(handler);
        }

        /// <summary>
        /// Initializes this instance using the specified events.
        /// </summary>
        /// <param name="events">The events to initialize with.</param>
        /// <exception cref="System.ArgumentNullException">events</exception>
        /// <exception cref="System.InvalidOperationException">Initialize cannot be called on an instance with changes.</exception>
        public void Initialize(IEnumerable<object> events)
        {
            if (events == null) throw new ArgumentNullException("events");
            //ThrowIfDisposed();
            if (HasChanges())
                throw new InvalidOperationException("Initialize cannot be called on an instance with changes.");
            foreach (var @event in events)
            {
                Play(@event);
            }
        }

        /// <summary>
        /// Applies the specified event to this instance and invokes the associated state handler.
        /// </summary>
        /// <param name="event">The event to apply.</param>
        protected void Apply(object @event)
        {
            if (@event == null) throw new ArgumentNullException("event");
            //ThrowIfDisposed();
            BeforeApply(@event);
            Play(@event);
            Record(@event);
            AfterApply(@event);
        }

        /// <summary>
        /// Called before an event is applied, exposed as a point of interception.
        /// </summary>
        /// <param name="event">The event that will be applied.</param>
        protected virtual void BeforeApply(object @event) {}

        /// <summary>
        /// Called after an event has been applied, exposed as a point of interception.
        /// </summary>
        /// <param name="event">The event that has been applied.</param>
        protected virtual void AfterApply(object @event) {}

        void Play(object @event)
        {
            _router.Route(@event);
        }

        void Record(object @event)
        {
            _recorder.Record(@event);

            ImmutableObserverList observers;
            lock (_gate)
            {
                ThrowIfDisposed();
                observers = _observers;
            }
            if (observers.IsEmpty) return;
            foreach (IObserver<object> observer in observers)
                observer.OnNext(@event);
        }

        void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        /// <summary>
        /// Determines whether this instance has state changes.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance has state changes; otherwise, <c>false</c>.
        /// </returns>
        public bool HasChanges()
        {
            //ThrowIfDisposed();
            return _recorder.Any();
        }

        /// <summary>
        /// Gets the state changes applied to this instance.
        /// </summary>
        /// <returns>
        /// A list of recorded state changes.
        /// </returns>
        public IEnumerable<object> GetChanges()
        {
            //ThrowIfDisposed();
            return _recorder.ToArray();
        }

        /// <summary>
        /// Clears the state changes.
        /// </summary>
        public void ClearChanges()
        {
            //ThrowIfDisposed();
            _recorder.Reset();
        }

        /// <summary>
        /// Subscribes the specified observer to this instance.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <returns>A disposable subscription.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="observer"/> is <c>null</c>.</exception>
        public IDisposable Subscribe(IObserver<object> observer)
        {
            if (observer == null) throw new ArgumentNullException("observer");
            lock (_gate)
            {
                ThrowIfDisposed();
                _observers = _observers.Add(observer);
                foreach (var change in GetChanges())
                {
                    observer.OnNext(change);
                }
                return new Subscription(this, observer);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            lock (_gate)
            {
                _disposed = true;
                _observers = ImmutableObserverList.Empty;
            }
        }

        class Subscription : IDisposable
        {
            readonly ObservableAggregateRootEntity _owner;
            IObserver<object> _observer;

            public Subscription(ObservableAggregateRootEntity owner, IObserver<object> observer)
            {
                _owner = owner;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer == null) return;
                lock (_owner._gate)
                {
                    if (_owner._disposed || _observer == null) return;
                    _owner._observers = _owner._observers.Remove(_observer);
                    _observer = null;
                }
            }
        }

        class ImmutableObserverList : IEnumerable
        {
            public static readonly ImmutableObserverList Empty = new ImmutableObserverList();

            readonly IObserver<object>[] _data;

            ImmutableObserverList()
            {
                _data = new IObserver<object>[0];
            }

            ImmutableObserverList(IObserver<object>[] data)
            {
                _data = data;
            }

            public bool IsEmpty
            {
                get { return ReferenceEquals(this, Empty); }
            }

            public ImmutableObserverList Add(IObserver<object> value)
            {
                var data = new IObserver<object>[_data.Length + 1];
                Array.Copy(_data, data, _data.Length);
                data[_data.Length] = value;
                return new ImmutableObserverList(data);
            }

            public ImmutableObserverList Remove(IObserver<object> value)
            {
                var index = IndexOf(value);
                if (index < 0)
                    return this;
                var data = new IObserver<object>[_data.Length - 1];
                Array.Copy(_data, 0, data, 0, index);
                Array.Copy(_data, index + 1, data, index, _data.Length - index - 1);
                return new ImmutableObserverList(data);
            }

            int IndexOf(IObserver<object> value)
            {
                for (var index = 0; index < _data.Length; ++index)
                {
                    if (_data[index].Equals(value))
                        return index;
                }
                return -1;
            }

            public IEnumerator GetEnumerator()
            {
                return _data.GetEnumerator();
            }
        }
    }
}