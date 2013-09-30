namespace AggregateSource.Testing
{
    /// <summary>
    /// The act of building an event-centric aggregate factory test specification.
    /// </summary>
    public interface IEventCentricAggregateFactoryTestSpecificationBuilder
    {
        /// <summary>
        /// Builds the test specification thus far.
        /// </summary>
        /// <returns>The test specification.</returns>
        EventCentricAggregateFactoryTestSpecification Build();
    }
}