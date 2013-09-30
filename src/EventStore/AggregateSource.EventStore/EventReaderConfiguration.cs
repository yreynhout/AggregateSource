using System;

namespace AggregateSource.EventStore
{
    /// <summary>
    /// Represents configuration settings used during reading from the event store.
    /// </summary>
    public class EventReaderConfiguration
    {
        readonly SliceSize _sliceSize;
        readonly IEventDeserializer _deserializer;
        readonly IStreamNameResolver _streamNameResolver;
        readonly IStreamUserCredentialsResolver _streamUserCredentialsResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventReaderConfiguration"/> class.
        /// </summary>
        /// <param name="sliceSize">Size of the slice to read.</param>
        /// <param name="deserializer">The event deserializer to use.</param>
        /// <param name="streamNameResolver">The stream name resolver to use.</param>
        /// <param name="streamUserCredentialsResolver">The stream user credentials resolver to use.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="deserializer"/> or <paramref name="streamNameResolver"/> is <c>null</c>.</exception>
        public EventReaderConfiguration(SliceSize sliceSize, IEventDeserializer deserializer,
                                        IStreamNameResolver streamNameResolver,
                                        IStreamUserCredentialsResolver streamUserCredentialsResolver)
        {
            if (deserializer == null) throw new ArgumentNullException("deserializer");
            if (streamNameResolver == null) throw new ArgumentNullException("streamNameResolver");
            if (streamUserCredentialsResolver == null) throw new ArgumentNullException("streamUserCredentialsResolver");
            _sliceSize = sliceSize;
            _deserializer = deserializer;
            _streamNameResolver = streamNameResolver;
            _streamUserCredentialsResolver = streamUserCredentialsResolver;
        }

        /// <summary>
        /// Gets the size of the slice to read.
        /// </summary>
        /// <value>
        /// The size of the slice to read.
        /// </value>
        public SliceSize SliceSize
        {
            get { return _sliceSize; }
        }

        /// <summary>
        /// Gets the event deserializer.
        /// </summary>
        /// <value>
        /// The event deserializer.
        /// </value>
        public IEventDeserializer Deserializer
        {
            get { return _deserializer; }
        }

        /// <summary>
        /// Gets the stream name resolver.
        /// </summary>
        /// <value>The stream name resolver.</value>
        public IStreamNameResolver StreamNameResolver
        {
            get { return _streamNameResolver; }
        }

        /// <summary>
        /// Gets the stream user credentials resolver.
        /// </summary>
        /// <value>The stream user credentials resolver.</value>
        public IStreamUserCredentialsResolver StreamUserCredentialsResolver
        {
            get { return _streamUserCredentialsResolver; }
        }
    }
}