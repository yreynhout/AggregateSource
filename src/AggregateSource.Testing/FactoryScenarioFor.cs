using System;

namespace AggregateSource.Testing
{
    public class FactoryScenarioFor<TAggregateRoot> : IAggregateFactoryGivenStateBuilder<TAggregateRoot>
        where TAggregateRoot : IAggregateRootEntity
    {
        readonly Func<IAggregateRootEntity> _sutFactory;

        public FactoryScenarioFor(TAggregateRoot sut)
            : this(() => sut)
        {
        }

        public FactoryScenarioFor(Func<TAggregateRoot> sutFactory)
        {
            _sutFactory = () => sutFactory();
        }

        public IAggregateFactoryGivenStateBuilder<TAggregateRoot> Given(params object[] events)
        {
            if (events == null) throw new ArgumentNullException("events");
            return new AggregateFactoryGivenStateBuilder<TAggregateRoot>(_sutFactory, events);
        }

        public IAggregateFactoryWhenStateBuilder When<TAggregateRootResult>(
            Func<TAggregateRoot, TAggregateRootResult> factory) where TAggregateRootResult : IAggregateRootEntity
        {
            if (factory == null) throw new ArgumentNullException("factory");
            return new AggregateFactoryWhenStateBuilder(_sutFactory, new object[0],
                                                        root => factory((TAggregateRoot) root));
        }
    }
}