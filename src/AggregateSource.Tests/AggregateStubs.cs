using System;

namespace AggregateSource {
  public static class AggregateStubs {
    public static readonly Aggregate Stub1 =
      Create(Guid.NewGuid(), AggregateRootEntityStub.Factory());
    public static readonly Aggregate Stub2 =
      Create(Guid.NewGuid(), AggregateRootEntityStub.Factory());

    public static Aggregate Create<TAggregateRoot>(TAggregateRoot root)
      where TAggregateRoot : AggregateRootEntity {
      return new Aggregate(Guid.NewGuid(), 0, root);
    }

    public static Aggregate Create<TAggregateRoot>(Guid id, TAggregateRoot root)
      where TAggregateRoot : AggregateRootEntity {
      return new Aggregate(id, 0, root);
    }
  }
}