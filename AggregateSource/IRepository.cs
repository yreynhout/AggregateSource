using System;

namespace AggregateSource {
  public interface IRepository<TAggregateRoot> where TAggregateRoot : AggregateRootEntity {
    TAggregateRoot Get(Guid id);
    bool TryGet(Guid id, out TAggregateRoot root);
    void Add(Guid id, TAggregateRoot root);
  }
}