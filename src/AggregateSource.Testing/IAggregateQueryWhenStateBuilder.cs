using System;

namespace AggregateSource.Testing
{
    /// <summary>
    /// The when state within the test specification building process.
    /// </summary>
    public interface IAggregateQueryWhenStateBuilder<TResult>
    {
        /// <summary>
        /// Then a result is returned.
        /// </summary>
        /// <param name="result">The result that should be returned.</param>
        /// <returns>A builder continuation.</returns>
        IAggregateQueryThenStateBuilder Then(TResult result);

        /// <summary>
        /// Throws an exception.
        /// </summary>
        /// <param name="exception">The exception thrown.</param>
        /// <returns>A builder continuation.</returns>
        IAggregateQueryThrowStateBuilder Throws(Exception exception);
    }
}