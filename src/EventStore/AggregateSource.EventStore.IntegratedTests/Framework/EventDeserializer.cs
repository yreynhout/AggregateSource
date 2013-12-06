using System;
using System.Collections.Generic;
using System.IO;
using EventStore.ClientAPI;

namespace AggregateSource.EventStore.Framework
{
    public class EventDeserializer : IEventDeserializer
    {
        public IEnumerable<object> Deserialize(ResolvedEvent resolvedEvent)
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
            yield return instance;
        }
    }
}