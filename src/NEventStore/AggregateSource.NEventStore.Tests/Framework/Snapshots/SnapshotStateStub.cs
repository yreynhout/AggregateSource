namespace AggregateSource.NEventStore.Framework.Snapshots
{
    public class SnapshotStateStub
    {
        public int Value { get; private set; }

        public SnapshotStateStub() {}

        public SnapshotStateStub(int value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SnapshotStateStub);
        }

        bool Equals(SnapshotStateStub other)
        {
            return !ReferenceEquals(other, null) && Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Value.GetHashCode()*10 + 1;
            }
        }
    }
}