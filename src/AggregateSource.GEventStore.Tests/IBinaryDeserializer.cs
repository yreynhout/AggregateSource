using System.IO;

namespace AggregateSource.GEventStore {
  public interface IBinaryDeserializer {
    void Read(BinaryReader reader);
  }
}