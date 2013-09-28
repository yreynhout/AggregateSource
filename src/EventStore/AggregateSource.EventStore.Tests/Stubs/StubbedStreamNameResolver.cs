namespace AggregateSource.EventStore.Stubs
{
    public class StubbedStreamNameResolver : IStreamNameResolver
    {
        public static readonly IStreamNameResolver Instance = new StubbedStreamNameResolver();

        StubbedStreamNameResolver() {}

        public string Resolve(string identifier)
        {
            return null;
        }
    }
}