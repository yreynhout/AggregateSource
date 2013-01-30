using System;

namespace AggregateSource.Tests {
  public static class AggregateStubs {
    public static readonly Aggregate Stub1 =
      Create(Guid.NewGuid(), AggregateRootEntityStub.Factory());
    public static readonly Aggregate Stub2 =
      Create(Guid.NewGuid(), AggregateRootEntityStub.Factory());

    public static Aggregate Create<TAggregateRoot>(TAggregateRoot root)
      where TAggregateRoot : AggregateRootEntity {
      return new Aggregate(Guid.NewGuid(), Aggregate.InitialVersion, root);
    }

    public static Aggregate Create<TAggregateRoot>(Guid id, TAggregateRoot root)
      where TAggregateRoot : AggregateRootEntity {
      return new Aggregate(id, Aggregate.InitialVersion, root);
    }
  }
}