using System;

namespace AggregateSource.Testing
{
    /// <summary>
    /// The when state within the test specification building process.
    /// </summary>
    public interface IAggregateFactoryWhenStateBuilder
    {
        /// <summary>
        /// Then no events should have occurred.
        /// </summary>
        /// <returns>A builder continuation.</returns>
        IAggregateFactoryThenNoneStateBuilder ThenNone();

        /// <summary>
        /// Then events should have occurred.
        /// </summary>
        /// <param name="events">The events that should have occurred.</param>
        /// <returns>A builder continuation.</returns>
        IAggregateFactoryThenStateBuilder Then(params object[] events);

        /// <summary>
        /// Throws an exception.
        /// </summary>
        /// <param name="exception">The exception thrown.</param>
        /// <returns>A builder continuation.</returns>
        IAggregateFactoryThrowStateBuilder Throws(Exception exception);
    }
}