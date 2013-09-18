namespace AggregateSource.Testing
{
    /// <summary>
    /// The given state within the test specification building process.
    /// </summary>
    public interface IScenarioGivenStateBuilder
    {
        /// <summary>
        /// Given the following facts occured.
        /// </summary>
        /// <param name="facts">The facts that occurred.</param>
        /// <returns>A builder continuation.</returns>
        IScenarioGivenStateBuilder Given(params Fact[] facts);

        /// <summary>
        /// Given the following events occured.
        /// </summary>
        /// <param name="identifier">The aggregate identifier the events are to be associated with.</param>
        /// <param name="events">The events that occurred.</param>
        /// <returns>A builder continuation.</returns>
        IScenarioGivenStateBuilder Given(string identifier, params object[] events);

        /// <summary>
        /// When a command occurs.
        /// </summary>
        /// <param name="message">The command message.</param>
        /// <returns>A builder continuation.</returns>
        IScenarioWhenStateBuilder When(object message);
    }
}