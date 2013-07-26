using System;

namespace AggregateSource.Testing
{
    /// <summary>
    /// The given state within the test specification building process.
    /// </summary>
    public interface IAggregateFactoryGivenStateBuilder<out TAggregateRoot> where TAggregateRoot : IAggregateRootEntity
    {
        /// <summary>
        /// Given the following events occured.
        /// </summary>
        /// <param name="events">The events that occurred.</param>
        /// <returns>A builder continuation.</returns>
        IAggregateFactoryGivenStateBuilder<TAggregateRoot> Given(params object[] events);

        /// <summary>
        /// When an aggregate is created.
        /// </summary>
        /// <param name="factory">The factory method invocation on the sut.</param>
        /// <returns>A builder continuation.</returns>
        IAggregateFactoryWhenStateBuilder When<TAggregateRootResult>(Func<TAggregateRoot, TAggregateRootResult> factory)
            where TAggregateRootResult : IAggregateRootEntity;
    }
}