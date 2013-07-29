namespace AggregateSource.GEventStore.Framework
{
    public static class AsyncEventReaderFactory
    {
        public static AsyncEventReader Create()
        {
            return new AsyncEventReader(EmbeddedEventStore.Instance.Connection, EventReaderConfigurationFactory.Create());
        }
    }
}