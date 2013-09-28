using EventStore.ClientAPI;

namespace AggregateSource.EventStore.Stubs
{
    public class StubbedEventDeserializer : IEventDeserializer
    {
        public static readonly IEventDeserializer Instance = new StubbedEventDeserializer();

        StubbedEventDeserializer() {}

        public object Deserialize(ResolvedEvent resolvedEvent)
        {
            return null;
        }
    }
}