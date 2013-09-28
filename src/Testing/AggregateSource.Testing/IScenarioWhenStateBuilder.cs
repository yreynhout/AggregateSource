using System;

namespace AggregateSource.Testing
{
    /// <summary>
    /// The when state within the test specification building process.
    /// </summary>
    public interface IScenarioWhenStateBuilder : IEventCentricTestSpecificationBuilder
    {
        /// <summary>
        /// Then facts should have occurred.
        /// </summary>
        /// <param name="facts">The facts that should have occurred.</param>
        /// <returns>A builder continuation.</returns>
        IScenarioThenStateBuilder Then(params Fact[] facts);

        /// <summary>
        /// Then events should have occurred.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="events">The events that should have occurred.</param>
        /// <returns>A builder continuation.</returns>
        IScenarioThenStateBuilder Then(string identifier, params object[] events);

        /// <summary>
        /// Then no events should have occurred.
        /// </summary>
        /// <returns>A builder continuation.</returns>
        IScenarioThenNoneStateBuilder ThenNone();

        /// <summary>
        /// Throws an exception.
        /// </summary>
        /// <param name="exception">The exception thrown.</param>
        /// <returns>A builder continuation.</returns>
        IScenarioThrowStateBuilder Throws(Exception exception);
    }
}