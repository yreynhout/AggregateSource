namespace AggregateSource.Testing
{
    /// <summary>
    /// The act of building an exception-centric test specification.
    /// </summary>
    public interface IExceptionCentricTestSpecificationBuilder
    {
        /// <summary>
        /// Builds the test specification thus far.
        /// </summary>
        /// <returns>The test specification.</returns>
        ExceptionCentricTestSpecification Build();
    }
}