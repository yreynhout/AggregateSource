using System;

namespace AggregateSource.EventStorage {
  public struct AggregateBasedStreamName {
    readonly string _value;

    public AggregateBasedStreamName(Guid id, Type aggregateRootEntityType) {
      _value = string.Format("{0}{1}", aggregateRootEntityType.Name, id.ToString("N"));
    }

    public static implicit operator String(AggregateBasedStreamName name) {
      return name._value;
    }
  }
}