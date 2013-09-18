using System;

namespace AggregateSource.Testing
{
    /// <summary>
    /// The given none state within the test specification building process.
    /// </summary>
    public interface IAggregateFactoryGivenNoneStateBuilder<out TAggregateRoot> where TAggregateRoot : IAggregateRootEntity
    {
        /// <summary>
        /// When an aggregate is created.
        /// </summary>
        /// <param name="factory">The factory method invocation on the sut.</param>
        /// <returns>A builder continuation.</returns>
        IAggregateFactoryWhenStateBuilder When<TAggregateRootResult>(Func<TAggregateRoot, TAggregateRootResult> factory)
            where TAggregateRootResult : IAggregateRootEntity;
    }
}