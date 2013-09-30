namespace AggregateSource.EventStore.Snapshots
{
    /// <summary>
    /// Represents a snapshot of an aggregate at a particular version.
    /// </summary>
    public class Snapshot
    {
        readonly int _version;
        readonly object _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="Snapshot"/> class.
        /// </summary>
        /// <param name="version">The version at which the snapshot was taken.</param>
        /// <param name="state">The state object when the snapshot was taken.</param>
        public Snapshot(int version, object state)
        {
            _version = version;
            _state = state;
        }

        /// <summary>
        /// Gets the version at which the snapshot was taken.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public int Version
        {
            get { return _version; }
        }

        /// <summary>
        /// Gets the state object when the snapshot was taken.
        /// </summary>
        /// <value>
        /// The state object.
        /// </value>
        public object State
        {
            get { return _state; }
        }

        bool Equals(Snapshot other)
        {
            if (ReferenceEquals(other, null)) return false;
            return _version == other._version && Equals(_state, other._state);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Snapshot);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (_state == null)
                return _version;
            return _version ^ _state.GetHashCode();
        }
    }
}