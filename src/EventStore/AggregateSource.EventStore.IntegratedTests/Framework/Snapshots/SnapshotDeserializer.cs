using System;
using System.IO;
using AggregateSource.EventStore.Snapshots;
using EventStore.ClientAPI;

namespace AggregateSource.EventStore.Framework.Snapshots
{
    public class SnapshotDeserializer : ISnapshotDeserializer
    {
        public Snapshot Deserialize(ResolvedEvent resolvedEvent)
        {
            var type = Type.GetType(resolvedEvent.Event.EventType, true);
            var instance = Activator.CreateInstance(type);
            using (var stream = new MemoryStream(resolvedEvent.Event.Data))
            {
                using (var reader = new BinaryReader(stream))
                {
                    ((IBinaryDeserializer) instance).Read(reader);
                    return new Snapshot(BitConverter.ToInt32(resolvedEvent.Event.Metadata, 0), instance);
                }
            }
        }
    }
}