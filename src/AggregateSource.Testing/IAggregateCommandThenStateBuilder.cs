namespace AggregateSource.Testing
{
    /// <summary>
    /// The then state within the test specification building process.
    /// </summary>
    public interface IAggregateCommandThenStateBuilder : IEventCentricAggregateCommandTestSpecificationBuilder
    {
        /// <summary>
        /// Then events should have occurred.
        /// </summary>
        /// <param name="events">The events that should have occurred.</param>
        /// <returns>A builder continuation.</returns>
        IAggregateCommandThenStateBuilder Then(params object[] events);
    }
}