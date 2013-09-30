using System;

namespace AggregateSource.Testing
{
    /// <summary>
    /// The given none state within the test specification building process.
    /// </summary>
    public interface IAggregateQueryGivenNoneStateBuilder<out TAggregateRoot> where TAggregateRoot : IAggregateRootEntity
    {
        /// <summary>
        /// When a query occurs.
        /// </summary>
        /// <param name="query">The query method invocation on the sut.</param>
        /// <returns>A builder continuation.</returns>
        IAggregateQueryWhenStateBuilder<TResult> When<TResult>(Func<TAggregateRoot, TResult> query);
    }
}