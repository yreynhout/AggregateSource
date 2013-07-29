namespace AggregateSource.Testing
{
    public interface IEventCentricTestSpecificationBuilder
    {
        /// <summary>
        /// Builds the test specification thus far.
        /// </summary>
        /// <returns>The test specification.</returns>
        EventCentricTestSpecification Build();
    }
}