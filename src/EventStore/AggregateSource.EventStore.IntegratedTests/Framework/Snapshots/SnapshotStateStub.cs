using System.IO;

namespace AggregateSource.EventStore.Framework.Snapshots
{
    public class SnapshotStateStub : IBinarySerializer, IBinaryDeserializer
    {
        int _value;

        public SnapshotStateStub() {}

        public SnapshotStateStub(int value)
        {
            _value = value;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(_value);
        }

        public void Read(BinaryReader reader)
        {
            _value = reader.ReadInt32();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SnapshotStateStub);
        }

        bool Equals(SnapshotStateStub other)
        {
            return !ReferenceEquals(other, null) && _value.Equals(other._value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return _value.GetHashCode()*10 + 1;
            }
        }
    }
}