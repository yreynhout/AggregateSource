using System;

namespace AggregateSource
{
    /// <summary>
    /// Routes an event to a configured state handler.
    /// </summary>
    public interface IStaticEventRouter
    {
        /// <summary>
        /// Routes the specified <paramref name="event"/> to a configured state handler, if any, on the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The instance to route to.</param>
        /// <param name="event">The event to route.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="instance"/> or <paramref name="event"/> is <c>null</c>.</exception>
        void Route(object instance, object @event);
    }
}