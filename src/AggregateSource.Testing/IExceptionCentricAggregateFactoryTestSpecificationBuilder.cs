namespace AggregateSource.Testing
{
    /// <summary>
    /// The act of building an exception-centric aggregate factory test specification.
    /// </summary>
    public interface IExceptionCentricAggregateFactoryTestSpecificationBuilder
    {
        /// <summary>
        /// Builds the test specification thus far.
        /// </summary>
        /// <returns>The test specification.</returns>
        ExceptionCentricAggregateFactoryTestSpecification Build();
    }
}