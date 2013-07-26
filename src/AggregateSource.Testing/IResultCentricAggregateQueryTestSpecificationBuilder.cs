namespace AggregateSource.Testing
{
    /// <summary>
    /// The act of building an event-centric aggregate query test specification.
    /// </summary>
    public interface IResultCentricAggregateQueryTestSpecificationBuilder
    {
        /// <summary>
        /// Builds the test specification thus far.
        /// </summary>
        /// <returns>The test specification.</returns>
        ResultCentricAggregateQueryTestSpecification Build();
    }
}