using System;

namespace AggregateSource.Testing {
  public class FactoryScenarioFor<TAggregateRoot> where TAggregateRoot : IAggregateRootEntity {
    readonly Func<IAggregateRootEntity> _sutFactory;

    public FactoryScenarioFor(TAggregateRoot sut)
      : this(() => sut) {}

    public FactoryScenarioFor(Func<TAggregateRoot> sutFactory) {
      _sutFactory = () => sutFactory();
    }

    public IAggregateFactoryGivenStateBuilder<TAggregateRoot> Given(params object[] events) {
      if (events == null) throw new ArgumentNullException("events");
      return new AggregateFactoryGivenStateBuilder<TAggregateRoot>(_sutFactory, events);
    }
  }
}