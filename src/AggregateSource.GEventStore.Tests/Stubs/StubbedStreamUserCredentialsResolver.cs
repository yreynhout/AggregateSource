using EventStore.ClientAPI.SystemData;

namespace AggregateSource.GEventStore.Stubs
{
    public class StubbedStreamUserCredentialsResolver : IStreamUserCredentialsResolver
    {
        public static readonly IStreamUserCredentialsResolver Instance = new StubbedStreamUserCredentialsResolver();

        StubbedStreamUserCredentialsResolver() {}

        public UserCredentials Resolve(string identifier)
        {
            return new UserCredentials("", "");
        }
    }
}