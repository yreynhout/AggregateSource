using System;
using System.IO;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore.Framework
{
    public class EventDeserializer : IEventDeserializer
    {
        public object Deserialize(ResolvedEvent resolvedEvent)
        {
            var instance =
                (IBinaryDeserializer)
                Activator.CreateInstance(Type.GetType(resolvedEvent.OriginalEvent.EventType, true));
            using (var stream = new MemoryStream(resolvedEvent.Event.Data))
            {
                using (var reader = new BinaryReader(stream))
                {
                    instance.Read(reader);
                }
            }
            return instance;
        }
    }
}