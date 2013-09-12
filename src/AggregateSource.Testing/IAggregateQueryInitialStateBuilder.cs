using System;

namespace AggregateSource.Testing
{
    /// <summary>
    /// The initial state within the test specification building process.
    /// </summary>
    public interface IAggregateQueryInitialStateBuilder<out TAggregateRoot> where TAggregateRoot : IAggregateRootEntity
    {
        /// <summary>
        /// Given no events occured.
        /// </summary>
        /// <returns>A builder continuation.</returns>
        IAggregateQueryGivenNoneStateBuilder<TAggregateRoot> GivenNone();

        /// <summary>
        /// Given the following events occured.
        /// </summary>
        /// <param name="events">The events that occurred.</param>
        /// <returns>A builder continuation.</returns>
        IAggregateQueryGivenStateBuilder<TAggregateRoot> Given(params object[] events);

        /// <summary>
        /// When a query occurs.
        /// </summary>
        /// <param name="query">The query method invocation on the sut.</param>
        /// <returns>A builder continuation.</returns>
        IAggregateQueryWhenStateBuilder<TResult> When<TResult>(Func<TAggregateRoot, TResult> query);
    }
}