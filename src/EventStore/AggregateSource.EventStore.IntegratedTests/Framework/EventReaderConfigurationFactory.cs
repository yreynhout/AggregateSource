using AggregateSource.EventStore.Resolvers;
using EventStore.ClientAPI.SystemData;

namespace AggregateSource.EventStore.Framework
{
    public static class EventReaderConfigurationFactory
    {
        public static EventReaderConfiguration Create()
        {
            return new EventReaderConfiguration(new SliceSize(1), new EventDeserializer(),
                                                new PassThroughStreamNameResolver(),
                                                new FixedStreamUserCredentialsResolver(new UserCredentials("admin",
                                                                                                           "changeit")));
        }

        public static EventReaderConfiguration CreateWithResolver(IStreamNameResolver resolver)
        {
            return new EventReaderConfiguration(new SliceSize(1), new EventDeserializer(), resolver,
                                                new FixedStreamUserCredentialsResolver(new UserCredentials("admin",
                                                                                                           "changeit")));
        }
    }
}