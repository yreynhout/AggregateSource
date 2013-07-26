namespace AggregateSource.Testing
{
    /// <summary>
    /// The act of building an event-centric test specification.
    /// </summary>
    public interface IEventCentricTestSpecificationBuilder
    {
        /// <summary>
        /// Builds the test specification thus far.
        /// </summary>
        /// <returns>The test specification.</returns>
        EventCentricTestSpecification Build();
    }
}