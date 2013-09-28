using EventStore.ClientAPI.SystemData;

namespace AggregateSource.EventStore.Stubs
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