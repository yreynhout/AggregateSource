namespace AggregateSource.Testing
{
    /// <summary>
    /// The then state within the test specification building process.
    /// </summary>
    public interface IScenarioThenStateBuilder : IEventCentricTestSpecificationBuilder
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
        /// <param name="identifier">The aggregate identifier the events belong to.</param>
        /// <param name="events">The events that should have occurred.</param>
        /// <returns>A builder continuation.</returns>
        IScenarioThenStateBuilder Then(string identifier, params object[] events);
    }
}