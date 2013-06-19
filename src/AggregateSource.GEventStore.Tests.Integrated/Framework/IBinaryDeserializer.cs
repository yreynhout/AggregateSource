using System.IO;

namespace AggregateSource.GEventStore.Framework {
  public interface IBinaryDeserializer {
    void Read(BinaryReader reader);
  }
}