namespace AggregateSource.EventStore.Builders
{
    public class EventReaderConfigurationBuilder
    {
        readonly SliceSize _sliceSize;
        readonly IEventDeserializer _deserializer;
        readonly IStreamNameResolver _streamNameResolver;
        readonly IStreamUserCredentialsResolver _streamUserCredentialsResolver;

        public static readonly EventReaderConfigurationBuilder Default = new EventReaderConfigurationBuilder();

        EventReaderConfigurationBuilder()
            : this(
                new SliceSize(1),
                Stubs.StubbedEventDeserializer.Instance,
                Stubs.StubbedStreamNameResolver.Instance,
                Stubs.StubbedStreamUserCredentialsResolver.Instance) {}

        EventReaderConfigurationBuilder(
            SliceSize sliceSize,
            IEventDeserializer deserializer,
            IStreamNameResolver streamNameResolver,
            IStreamUserCredentialsResolver streamUserCredentialsResolver)
        {
            _sliceSize = sliceSize;
            _deserializer = deserializer;
            _streamNameResolver = streamNameResolver;
            _streamUserCredentialsResolver = streamUserCredentialsResolver;
        }

        public EventReaderConfigurationBuilder UsingSliceSize(SliceSize value)
        {
            return new EventReaderConfigurationBuilder(value, _deserializer, _streamNameResolver,
                                                       _streamUserCredentialsResolver);
        }

        public EventReaderConfigurationBuilder UsingDeserializer(IEventDeserializer value)
        {
            return new EventReaderConfigurationBuilder(_sliceSize, value, _streamNameResolver,
                                                       _streamUserCredentialsResolver);
        }

        public EventReaderConfigurationBuilder UsingStreamNameResolver(IStreamNameResolver value)
        {
            return new EventReaderConfigurationBuilder(_sliceSize, _deserializer, value, _streamUserCredentialsResolver);
        }

        public EventReaderConfigurationBuilder UsingStreamUserCredentialsResolver(IStreamUserCredentialsResolver value)
        {
            return new EventReaderConfigurationBuilder(_sliceSize, _deserializer, _streamNameResolver, value);
        }

        public EventReaderConfiguration Build()
        {
            return new EventReaderConfiguration(_sliceSize, _deserializer, _streamNameResolver,
                                                _streamUserCredentialsResolver);
        }

        public static implicit operator EventReaderConfiguration(EventReaderConfigurationBuilder builder)
        {
            return builder.Build();
        }
    }
}