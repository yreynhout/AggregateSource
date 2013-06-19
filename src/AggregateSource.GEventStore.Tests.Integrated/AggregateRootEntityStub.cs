using System;

namespace AggregateSource.GEventStore {
  public class AggregateRootEntityStub : AggregateRootEntity {
    public static readonly Func<AggregateRootEntityStub> Factory = () => new AggregateRootEntityStub();
  }
}