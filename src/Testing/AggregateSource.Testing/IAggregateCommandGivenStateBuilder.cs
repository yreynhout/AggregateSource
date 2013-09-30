using System;

namespace AggregateSource.Testing
{
    /// <summary>
    /// The given state within the test specification building process.
    /// </summary>
    public interface IAggregateCommandGivenStateBuilder<out TAggregateRoot> where TAggregateRoot : IAggregateRootEntity
    {
        /// <summary>
        /// Given the following events occured.
        /// </summary>
        /// <param name="events">The events that occurred.</param>
        /// <returns>A builder continuation.</returns>
        IAggregateCommandGivenStateBuilder<TAggregateRoot> Given(params object[] events);

        /// <summary>
        /// When a command occurs.
        /// </summary>
        /// <param name="command">The command method invocation on the sut.</param>
        /// <returns>A builder continuation.</returns>
        IAggregateCommandWhenStateBuilder When(Action<TAggregateRoot> command);
    }
}