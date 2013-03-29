using System;

namespace AggregateSource.Testing {
  public interface IAggregateFactoryGivenStateBuilder<out TAggregateRoot> where TAggregateRoot : IAggregateRootEntity {
    IAggregateFactoryGivenStateBuilder<TAggregateRoot> Given(params object[] events);

    IAggregateFactoryWhenStateBuilder When<TAggregateRootResult>(Func<TAggregateRoot, TAggregateRootResult> factory)
      where TAggregateRootResult : IAggregateRootEntity;
  }
}