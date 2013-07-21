using AggregateSource.GEventStore.Snapshots;

namespace AggregateSource.GEventStore.Builders {
  public class SnapshotStoreReadConfigurationBuilder {
    readonly ISnapshotDeserializer _deserializer;
    readonly IStreamNameResolver _streamNameResolver;
    readonly IStreamUserCredentialsResolver _streamUserCredentialsResolver;

    public static readonly SnapshotStoreReadConfigurationBuilder Default = new SnapshotStoreReadConfigurationBuilder();

    SnapshotStoreReadConfigurationBuilder()
      : this(
          Stubs.StubbedSnapshotDeserializer.Instance, 
          Stubs.StubbedStreamNameResolver.Instance,
          Stubs.StubbedStreamUserCredentialsResolver.Instance) {}

    SnapshotStoreReadConfigurationBuilder(
      ISnapshotDeserializer deserializer, 
      IStreamNameResolver streamNameResolver,
      IStreamUserCredentialsResolver streamUserCredentialsResolver) {
      _deserializer = deserializer;
      _streamNameResolver = streamNameResolver;
      _streamUserCredentialsResolver = streamUserCredentialsResolver;
    }

    public SnapshotStoreReadConfigurationBuilder UsingDeserializer(ISnapshotDeserializer value) {
      return new SnapshotStoreReadConfigurationBuilder(value, _streamNameResolver, _streamUserCredentialsResolver);
    }

    public SnapshotStoreReadConfigurationBuilder UsingStreamNameResolver(IStreamNameResolver value) {
      return new SnapshotStoreReadConfigurationBuilder(_deserializer, value, _streamUserCredentialsResolver);
    }

    public SnapshotStoreReadConfigurationBuilder UsingStreamUserCredentialsResolver(IStreamUserCredentialsResolver value) {
      return new SnapshotStoreReadConfigurationBuilder(_deserializer, _streamNameResolver, value);
    }

    public SnapshotStoreReadConfiguration Build() {
      return new SnapshotStoreReadConfiguration(_deserializer, _streamNameResolver, _streamUserCredentialsResolver);
    }

    public static implicit operator SnapshotStoreReadConfiguration(SnapshotStoreReadConfigurationBuilder builder) {
      return builder.Build();
    }
  }
}
