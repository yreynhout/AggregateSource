using System;
using AggregateSource.GEventStore.Properties;

namespace AggregateSource.GEventStore {
  public struct SliceSize {
    readonly int _value;

    public SliceSize(int value) {
      if(value < 1)
        throw new ArgumentOutOfRangeException("value", Resources.SliceSize_GreaterThanOne);
      _value = value;
    }

    public static implicit operator Int32(SliceSize size) {
      return size._value;
    }
  }
}