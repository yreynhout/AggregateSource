using System;

namespace AggregateSource.GEventStore {
  public struct StreamName {
    readonly string _value;

    StreamName(Guid id, Type type) {
      _value = string.Format("{0}_{1}", type.Name, id.ToString("N"));
    }

    public static implicit operator string(StreamName name) {
      return name._value;
    }

    public static StreamName Create<TAggregateRoot>(Guid id) {
      return new StreamName(id, typeof(TAggregateRoot));
    }

    public static StreamName Create(Guid id, Type type) {
      if (type == null) throw new ArgumentNullException("type");
      return new StreamName(id, type);
    }
  }
}