using System;

namespace AggregateSource.Testing
{
    /// <summary>
    /// The given none state within the test specification building process.
    /// </summary>
    public interface IAggregateCommandGivenNoneStateBuilder<out TAggregateRoot>
        where TAggregateRoot : IAggregateRootEntity
    {
        /// <summary>
        /// When a command occurs.
        /// </summary>
        /// <param name="command">The command method invocation on the sut.</param>
        /// <returns>A builder continuation.</returns>
        IAggregateCommandWhenStateBuilder When(Action<TAggregateRoot> command);
    }
}