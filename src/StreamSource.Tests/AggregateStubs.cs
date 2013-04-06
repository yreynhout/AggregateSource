using System;
using AggregateSource;

namespace StreamSource {
  public static class AggregateStubs {
    public static readonly Aggregate Stub1 =
      Create(Guid.NewGuid().ToString(), AggregateRootEntityStub.Factory());
    public static readonly Aggregate Stub2 =
      Create(Guid.NewGuid().ToString(), AggregateRootEntityStub.Factory());

    public static Aggregate Create<TAggregateRoot>(TAggregateRoot root)
      where TAggregateRoot : AggregateRootEntity {
      return new Aggregate(Guid.NewGuid().ToString(), 0, root);
    }

    public static Aggregate Create<TAggregateRoot>(string identifier, TAggregateRoot root)
      where TAggregateRoot : AggregateRootEntity {
      return new Aggregate(identifier, 0, root);
    }
  }
}