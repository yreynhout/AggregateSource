using System;
using System.Collections.Generic;

namespace AggregateSource
{
    /// <summary>
    /// Routes an event to a configured state handler.
    /// </summary>
    public class StaticEventRouter : IConfigureStaticEventRouter
    {
        readonly Dictionary<Type, Dictionary<Type, Action<object, object>>> _index;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticEventRouter"/> class.
        /// </summary>
        public StaticEventRouter()
        {
            _index = new Dictionary<Type, Dictionary<Type, Action<object, object>>>();
        }

        /// <summary>
        /// Adds a route for the specified event type to the specified state handler.
        /// </summary>
        /// <typeparam name="TInstance">The instance type this route is for.</typeparam>
        /// <typeparam name="TEvent">The event type this route is for.</typeparam>
        /// <param name="handler">The state handler that should be invoked when an event of the specified type is routed to an instance of the specified type.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="handler" /> is <c>null</c>.</exception>
        public void ConfigureRoute<TInstance, TEvent>(Action<TInstance, TEvent> handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            Dictionary<Type, Action<object, object>> handlers;
            if (!_index.TryGetValue(typeof(TInstance), out handlers))
            {
                handlers = new Dictionary<Type, Action<object, object>>();
                _index.Add(typeof(TInstance), handlers);
            }
            handlers.Add(typeof(TEvent), (instance, @event) => handler((TInstance)instance, (TEvent)@event));
        }

        /// <summary>
        /// Adds a route for the specified event type to the specified state handler.
        /// </summary>
        /// <param name="instance">The instance type the route is for.</param>
        /// <param name="event">The event type the route is for.</param>
        /// <param name="handler">The state handler that should be invoked when an event of the specified type is routed.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> or <paramref name="event"/> or <paramref name="handler"/> is <c>null</c>.</exception>
        public void ConfigureRoute(Type instance, Type @event, Action<object, object> handler)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (@event == null) throw new ArgumentNullException("event");
            if (handler == null) throw new ArgumentNullException("handler");
            Dictionary<Type, Action<object, object>> handlers;
            if (!_index.TryGetValue(instance, out handlers))
            {
                handlers = new Dictionary<Type, Action<object, object>>();
                _index.Add(instance, handlers);
            }
            handlers.Add(@event, handler);
        }

        /// <summary>
        /// Routes the specified <paramref name="event"/> to a configured state handler, if any, on the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The instance to route to.</param>
        /// <param name="event">The event to route.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="instance"/> or <paramref name="event"/> is <c>null</c>.</exception>
        public void Route(object instance, object @event)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (@event == null) throw new ArgumentNullException("event");
            Dictionary<Type, Action<object, object>> handlers;
            if (!_index.TryGetValue(instance.GetType(), out handlers)) return;
            Action<object, object> handler;
            if (handlers.TryGetValue(@event.GetType(), out handler))
            {
                handler(instance, @event);
            }
        }
    }
}