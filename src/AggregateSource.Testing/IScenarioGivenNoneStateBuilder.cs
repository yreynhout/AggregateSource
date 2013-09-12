namespace AggregateSource.Testing
{
    /// <summary>
    /// The given none state within the test specification building process.
    /// </summary>
    public interface IScenarioGivenNoneStateBuilder
    {
        /// <summary>
        /// When a command occurs.
        /// </summary>
        /// <param name="message">The command message.</param>
        /// <returns>A builder continuation.</returns>
        IScenarioWhenStateBuilder When(object message);
    }
}