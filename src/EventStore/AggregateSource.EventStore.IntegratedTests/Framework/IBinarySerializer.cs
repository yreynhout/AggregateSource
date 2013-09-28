using System.IO;

namespace AggregateSource.EventStore.Framework
{
    public interface IBinarySerializer
    {
        void Write(BinaryWriter writer);
    }
}