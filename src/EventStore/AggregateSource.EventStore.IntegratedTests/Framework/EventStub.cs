using System.IO;

namespace AggregateSource.EventStore.Framework
{
    public class EventStub : IBinarySerializer, IBinaryDeserializer
    {
        int _value;

        public EventStub() {}

        public EventStub(int value)
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
            return Equals(obj as EventStub);
        }

        bool Equals(EventStub @event)
        {
            return !ReferenceEquals(@event, null) && _value.Equals(@event._value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return _value.GetHashCode()*10 + 2;
            }
        }
    }
}