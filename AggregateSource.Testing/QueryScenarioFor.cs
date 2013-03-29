using System;

namespace AggregateSource.Testing {
  public class QueryScenarioFor<TAggregateRoot> where TAggregateRoot : IAggregateRootEntity {
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
  }
}