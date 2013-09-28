using AggregateSource.EventStore.Snapshots;

namespace AggregateSource.EventStore.Builders
{
    public class SnapshotReaderConfigurationBuilder
    {
        readonly ISnapshotDeserializer _deserializer;
        readonly IStreamNameResolver _streamNameResolver;
        readonly IStreamUserCredentialsResolver _streamUserCredentialsResolver;

        public static readonly SnapshotReaderConfigurationBuilder Default = new SnapshotReaderConfigurationBuilder();

        SnapshotReaderConfigurationBuilder()
            : this(
                Stubs.StubbedSnapshotDeserializer.Instance,
                Stubs.StubbedStreamNameResolver.Instance,
                Stubs.StubbedStreamUserCredentialsResolver.Instance) {}

        SnapshotReaderConfigurationBuilder(
            ISnapshotDeserializer deserializer,
            IStreamNameResolver streamNameResolver,
            IStreamUserCredentialsResolver streamUserCredentialsResolver)
        {
            _deserializer = deserializer;
            _streamNameResolver = streamNameResolver;
            _streamUserCredentialsResolver = streamUserCredentialsResolver;
        }

        public SnapshotReaderConfigurationBuilder UsingDeserializer(ISnapshotDeserializer value)
        {
            return new SnapshotReaderConfigurationBuilder(value, _streamNameResolver, _streamUserCredentialsResolver);
        }

        public SnapshotReaderConfigurationBuilder UsingStreamNameResolver(IStreamNameResolver value)
        {
            return new SnapshotReaderConfigurationBuilder(_deserializer, value, _streamUserCredentialsResolver);
        }

        public SnapshotReaderConfigurationBuilder UsingStreamUserCredentialsResolver(
            IStreamUserCredentialsResolver value)
        {
            return new SnapshotReaderConfigurationBuilder(_deserializer, _streamNameResolver, value);
        }

        public SnapshotReaderConfiguration Build()
        {
            return new SnapshotReaderConfiguration(_deserializer, _streamNameResolver, _streamUserCredentialsResolver);
        }

        public static implicit operator SnapshotReaderConfiguration(SnapshotReaderConfigurationBuilder builder)
        {
            return builder.Build();
        }
    }
}