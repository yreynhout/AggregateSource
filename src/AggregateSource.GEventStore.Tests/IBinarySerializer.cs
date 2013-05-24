using System.IO;

namespace AggregateSource.GEventStore {
  public interface IBinarySerializer {
    void Write(BinaryWriter writer);
  }
}