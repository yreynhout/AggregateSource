using System;

namespace AggregateSource.Testing {
  public class QueryScenarioFor<TAggregateRoot> : IAggregateQueryGivenStateBuilder<TAggregateRoot> where TAggregateRoot : IAggregateRootEntity {
    readonly Func<IAggregateRootEntity> _sutFactory;

    public QueryScenarioFor(TAggregateRoot sut)
      : this(() => sut) {}

    public QueryScenarioFor(Func<TAggregateRoot> sutFactory) {
      _sutFactory = () => sutFactory();
    }

    public IAggregateQueryGivenStateBuilder<TAggregateRoot> Given(params object[] events) {
      if (events == null) throw new ArgumentNullException("events");
      return new AggregateQueryGivenStateBuilder<TAggregateRoot>(_sutFactory, events);
    }

    public IAggregateQueryWhenStateBuilder<TResult> When<TResult>(Func<TAggregateRoot, TResult> query) {
      if (query == null) throw new ArgumentNullException("query");
      return new AggregateQueryWhenStateBuilder<TResult>(_sutFactory, new object[0], root => query((TAggregateRoot)root));
    }
  }
}