using System;

namespace AggregateSource
{
    /// <summary>
    /// Base class for aggregate state objects that need some basic infrastructure for routing events to handlers.
    /// </summary>
    public abstract class EntityState : IInstanceEventRouter
    {
        readonly InstanceEventRouter _router;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityState"/> class.
        /// </summary>
        protected EntityState()
        {
            _router = new InstanceEventRouter();
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
        /// Routes the specified <paramref name="event"/> to a configured state handler, if any.
        /// </summary>
        /// <param name="event">The event to route.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="event"/> is null.</exception>
        public void Route(object @event)
        {
            if (@event == null) throw new ArgumentNullException("event");
            _router.Route(@event);
        }
    }
}