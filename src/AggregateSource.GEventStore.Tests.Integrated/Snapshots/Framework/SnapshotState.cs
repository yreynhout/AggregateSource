using System.IO;
using AggregateSource.GEventStore.Framework;

namespace AggregateSource.GEventStore.Snapshots.Framework {
  public class SnapshotState : IBinarySerializer, IBinaryDeserializer {
    string _value;

    public SnapshotState() {
      _value = "snapshot";
    }

    public void Write(BinaryWriter writer) {
      writer.Write(_value);
    }

    public void Read(BinaryReader reader) {
      _value = reader.ReadString();
    }

    public override bool Equals(object obj) {
      return Equals(obj as SnapshotState);
    }

    bool Equals(SnapshotState other) {
      return !ReferenceEquals(other, null) && _value.Equals(other._value);
    }

    public override int GetHashCode() {
      return _value.GetHashCode();
    }
  }
}