namespace AggregateSource.Testing
{
    /// <summary>
    /// The act of building an exception-centric aggregate query test specification.
    /// </summary>
    public interface IExceptionCentricAggregateQueryTestSpecificationBuilder
    {
        /// <summary>
        /// Builds the test specification thus far.
        /// </summary>
        /// <returns>The test specification.</returns>
        ExceptionCentricAggregateQueryTestSpecification Build();
    }
}