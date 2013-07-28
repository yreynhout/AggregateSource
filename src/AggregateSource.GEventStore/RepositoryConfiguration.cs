namespace AggregateSource.GEventStore
{
    /// <summary>
    /// Represents configuration settings used during reading from a repository.
    /// </summary>
    public class RepositoryConfiguration
    {
        readonly bool _requireStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryConfiguration"/> class.
        /// </summary>
        /// <param name="requireStream">if set to <c>true</c> requires a stream to be present for the aggregate being read.</param>
        public RepositoryConfiguration(bool requireStream)
        {
            _requireStream = requireStream;
        }

        /// <summary>
        /// Gets a value indicating whether a stream is required while reading an aggregate.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a stream is required; otherwise, <c>false</c>.
        /// </value>
        public bool RequireStream
        {
            get { return _requireStream; }
        }
    }
}