using System;

namespace AggregateSource
{
    /// <summary>
    /// Configures a static event router with state handlers events can be routed to.
    /// </summary>
    public interface IConfigureStaticEventRouter : IStaticEventRouter
    {
        /// <summary>
        /// Adds a route for the specified event type to the specified state handler.
        /// </summary>
        /// <typeparam name="TInstance">The instance type this route is for.</typeparam>
        /// <typeparam name="TEvent">The event type this route is for.</typeparam>
        /// <param name="handler">The state handler that should be invoked when an event of the specified type is routed to an instance of the specified type.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="handler" /> is <c>null</c>.</exception>
        void ConfigureRoute<TInstance, TEvent>(Action<TInstance, TEvent> handler);

        /// <summary>
        /// Adds a route for the specified event type to the specified state handler.
        /// </summary>
        /// <param name="instance">The instance type the route is for.</param>
        /// <param name="event">The event type the route is for.</param>
        /// <param name="handler">The state handler that should be invoked when an event of the specified type is routed.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> or <paramref name="event"/> or <paramref name="handler"/> is <c>null</c>.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "event")]
        void ConfigureRoute(Type instance, Type @event, Action<object, object> handler);
    }
}