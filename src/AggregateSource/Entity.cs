using System;
using System.Collections.Generic;

namespace AggregateSource
{
    /// <summary>
    /// Base class for aggregate entities that need some basic infrastructure for tracking state changes on their aggregate root entity.
    /// </summary>
    public abstract class Entity
    {
        readonly Action<object> _applier;
        readonly Dictionary<Type, Action<object>> _handlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        /// <param name="applier">The event player and recorder.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="applier"/> is null.</exception>
        protected Entity(Action<object> applier)
        {
            if (applier == null) throw new ArgumentNullException("applier");
            _applier = applier;
            _handlers = new Dictionary<Type, Action<object>>();
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
            _handlers.Add(typeof (TEvent), @event => handler((TEvent) @event));
        }

        /// <summary>
        /// Initializes this instance using the specified event.
        /// </summary>
        /// <param name="event">The event to initialize with.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="event"/> is null.</exception>
        public void Play(object @event)
        {
            if (@event == null) throw new ArgumentNullException("event");
            Action<object> handler;
            if (_handlers.TryGetValue(@event.GetType(), out handler))
            {
                handler(@event);
            }
        }

        /// <summary>
        /// Applies the specified event to this instance and invokes the associated state handler.
        /// </summary>
        /// <param name="event">The event to apply.</param>
        protected void Apply(object @event)
        {
            if (@event == null) throw new ArgumentNullException("event");
            _applier(@event);
        }
    }
}