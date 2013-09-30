using System.IO;

namespace AggregateSource.EventStore.Framework
{
    public interface IBinaryDeserializer
    {
        void Read(BinaryReader reader);
    }
}