using System;
using System.Collections.Generic;
using System.Linq;

namespace AggregateSource
{
    /// <summary>
    /// Base class for aggregate root entities that need some basic infrastructure for tracking state changes.
    /// </summary>
    public abstract class AggregateRootEntity : IAggregateRootEntity
    {
        readonly EventRecorder _recorder;
        readonly IConfigureInstanceEventRouter _router;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootEntity"/> class.
        /// </summary>
        protected AggregateRootEntity()
        {
            _router = new InstanceEventRouter();
            _recorder = new EventRecorder();
        }

        /// <summary>
        /// Registers the state handler to be invoked when the specified event is applied.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to register the handler for.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="handler"/> is null.</exception>
        protected void Register<TEvent>(Action<TEvent> handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            _router.ConfigureRoute(handler);
        }

        /// <summary>
        /// Initializes this instance using the specified events.
        /// </summary>
        /// <param name="events">The events to initialize with.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="events"/> are null.</exception>
        public void Initialize(IEnumerable<object> events)
        {
            if (events == null) throw new ArgumentNullException("events");
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
        protected void ApplyChange(object @event)
        {
            if (@event == null) throw new ArgumentNullException("event");
            BeforeApplyChange(@event);
            Play(@event);
            Record(@event);
            AfterApplyChange(@event);
        }

        /// <summary>
        /// Called before an event is applied, exposed as a point of interception.
        /// </summary>
        /// <param name="event">The event that will be applied.</param>
        protected virtual void BeforeApplyChange(object @event) {}

        /// <summary>
        /// Called after an event has been applied, exposed as a point of interception.
        /// </summary>
        /// <param name="event">The event that has been applied.</param>
        protected virtual void AfterApplyChange(object @event) {}

        void Play(object @event)
        {
            _router.Route(@event);
        }

        void Record(object @event)
        {
            _recorder.Record(@event);
        }

        /// <summary>
        /// Determines whether this instance has state changes.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance has state changes; otherwise, <c>false</c>.
        /// </returns>
        public bool HasChanges()
        {
            return _recorder.Any();
        }

        /// <summary>
        /// Gets the state changes applied to this instance.
        /// </summary>
        /// <returns>A list of recorded state changes.</returns>
        public IEnumerable<object> GetChanges()
        {
            return _recorder.ToArray();
        }

        /// <summary>
        /// Clears the state changes.
        /// </summary>
        public void ClearChanges()
        {
            _recorder.Reset();
        }
    }
}