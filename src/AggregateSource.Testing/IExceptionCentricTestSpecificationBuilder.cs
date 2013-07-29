namespace AggregateSource.Testing
{
    public interface IExceptionCentricTestSpecificationBuilder
    {
        /// <summary>
        /// Builds the test specification thus far.
        /// </summary>
        /// <returns>The test specification.</returns>
        ExceptionCentricTestSpecification Build();
    }
}