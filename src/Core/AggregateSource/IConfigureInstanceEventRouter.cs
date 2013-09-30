using System;

namespace AggregateSource
{
    /// <summary>
    /// Configures an instance event router with state handlers events can be routed to.
    /// </summary>
    public interface IConfigureInstanceEventRouter : IInstanceEventRouter
    {
        /// <summary>
        /// Adds a route for the specified event type to the specified state handler.
        /// </summary>
        /// <param name="event">The event type the route is for.</param>
        /// <param name="handler">The state handler that should be invoked when an event of the specified type is routed.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="event"/> or <paramref name="handler"/> is <c>null</c>.</exception>
        void ConfigureRoute(Type @event, Action<object> handler);

        /// <summary>
        /// Adds a route for the specified event type to the specified state handler.
        /// </summary>    
        /// <typeparam name="TEvent">The event type the route is for.</typeparam>
        /// <param name="handler">The state handler that should be invoked when an event of the specified type is routed.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="handler"/> is <c>null</c>.</exception>
        void ConfigureRoute<TEvent>(Action<TEvent> handler);
    }
}