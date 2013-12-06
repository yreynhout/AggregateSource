using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;

namespace AggregateSource.EventStore.Stubs
{
    public class StubbedEventDeserializer : IEventDeserializer
    {
        public static readonly IEventDeserializer Instance = new StubbedEventDeserializer();

        StubbedEventDeserializer() {}

        public IEnumerable<object> Deserialize(ResolvedEvent resolvedEvent)
        {
            return Enumerable.Empty<object>();
        }
    }
}