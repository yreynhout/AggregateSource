using System.IO;

namespace AggregateSource.GEventStore.Framework
{
    public interface IBinarySerializer
    {
        void Write(BinaryWriter writer);
    }
}