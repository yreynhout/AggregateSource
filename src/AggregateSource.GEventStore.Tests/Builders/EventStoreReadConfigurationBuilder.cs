namespace AggregateSource.GEventStore.Builders {
  public class EventStoreReadConfigurationBuilder {
    readonly SliceSize _sliceSize;
    readonly IEventDeserializer _deserializer;
    readonly IStreamNameResolver _streamNameResolver;
    readonly IStreamUserCredentialsResolver _streamUserCredentialsResolver;

    public static readonly EventStoreReadConfigurationBuilder Default = new EventStoreReadConfigurationBuilder();

    EventStoreReadConfigurationBuilder()
      : this(
        new SliceSize(1), 
        Stubs.StubbedEventDeserializer.Instance,
        Stubs.StubbedStreamNameResolver.Instance,
        Stubs.StubbedStreamUserCredentialsResolver.Instance) { }

    EventStoreReadConfigurationBuilder(
      SliceSize sliceSize,
      IEventDeserializer deserializer,
      IStreamNameResolver streamNameResolver,
      IStreamUserCredentialsResolver streamUserCredentialsResolver) {
      _sliceSize = sliceSize;
      _deserializer = deserializer;
      _streamNameResolver = streamNameResolver;
      _streamUserCredentialsResolver = streamUserCredentialsResolver;
    }

    public EventStoreReadConfigurationBuilder UsingSliceSize(SliceSize value) {
      return new EventStoreReadConfigurationBuilder(value, _deserializer, _streamNameResolver, _streamUserCredentialsResolver);
    }

    public EventStoreReadConfigurationBuilder UsingDeserializer(IEventDeserializer value) {
      return new EventStoreReadConfigurationBuilder(_sliceSize,value, _streamNameResolver, _streamUserCredentialsResolver);
    }

    public EventStoreReadConfigurationBuilder UsingStreamNameResolver(IStreamNameResolver value) {
      return new EventStoreReadConfigurationBuilder(_sliceSize, _deserializer, value, _streamUserCredentialsResolver);
    }

    public EventStoreReadConfigurationBuilder UsingStreamUserCredentialsResolver(IStreamUserCredentialsResolver value) {
      return new EventStoreReadConfigurationBuilder(_sliceSize, _deserializer, _streamNameResolver, value);
    }

    public EventStoreReadConfiguration Build() {
      return new EventStoreReadConfiguration(_sliceSize, _deserializer, _streamNameResolver, _streamUserCredentialsResolver);
    }

    public static implicit operator EventStoreReadConfiguration(EventStoreReadConfigurationBuilder builder) {
      return builder.Build();
    }
  }
}