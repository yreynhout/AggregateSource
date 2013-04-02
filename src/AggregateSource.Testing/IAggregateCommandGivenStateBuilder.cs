using System;

namespace AggregateSource.Testing {
  public interface IAggregateCommandGivenStateBuilder<out TAggregateRoot> where TAggregateRoot : IAggregateRootEntity {
    IAggregateCommandGivenStateBuilder<TAggregateRoot> Given(params object[] events);
    IAggregateCommandWhenStateBuilder When(Action<TAggregateRoot> command);
  }
}