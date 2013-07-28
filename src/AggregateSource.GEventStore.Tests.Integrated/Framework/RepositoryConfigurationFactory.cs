namespace AggregateSource.GEventStore.Framework
{
    public static class RepositoryConfigurationFactory
    {
        public static RepositoryConfiguration Create()
        {
            return new RepositoryConfiguration(true);
        }
    }
}