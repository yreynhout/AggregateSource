namespace AggregateSource.Testing
{
    /// <summary>
    /// The act of building an event-centric aggregate command test specification.
    /// </summary>
    public interface IEventCentricAggregateCommandTestSpecificationBuilder
    {
        /// <summary>
        /// Builds the test specification thus far.
        /// </summary>
        /// <returns>The test specification.</returns>
        EventCentricAggregateCommandTestSpecification Build();
    }
}