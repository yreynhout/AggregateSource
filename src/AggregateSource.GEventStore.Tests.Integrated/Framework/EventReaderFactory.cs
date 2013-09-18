namespace AggregateSource.GEventStore.Framework
{
    public static class EventReaderFactory
    {
        public static EventReader Create()
        {
            return new EventReader(EmbeddedEventStore.Connection, EventReaderConfigurationFactory.Create());
        }
    }
}
