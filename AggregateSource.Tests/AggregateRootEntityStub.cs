using System;

namespace AggregateSource.Tests {
  public class AggregateRootEntityStub : AggregateRootEntity {
    public static readonly Func<AggregateRootEntityStub> Factory = () => new AggregateRootEntityStub();
  }
}