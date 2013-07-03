namespace AggregateSource.GEventStore.Snapshots {
  /// <summary>
  /// 
  /// </summary>
  public class Snapshot {
    protected bool Equals(Snapshot other) {
      return _version == other._version && _state.Equals(other._state);
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((Snapshot) obj);
    }

    public override int GetHashCode() {
      unchecked {
        return (_version*397) ^ _state.GetHashCode();
      }
    }

    readonly int _version;
    readonly object _state;

    public Snapshot(int version, object state) {
      _version = version;
      _state = state;
    }

    public int Version {
      get { return _version; }
    }

    public object State {
      get { return _state; }
    }
  }
}