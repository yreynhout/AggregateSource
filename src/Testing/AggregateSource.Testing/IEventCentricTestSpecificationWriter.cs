namespace AggregateSource.Testing
{
    /// <summary>
    /// Writes test specifications for human consumption.
    /// </summary>
    public interface IEventCentricTestSpecificationWriter
    {
        /// <summary>
        /// Writes the specified test specification.
        /// </summary>
        /// <param name="specification">The test specification to write.</param>
        void Write(EventCentricTestSpecification specification);
    }
}