using System;

namespace AggregateSource.Testing
{
    public class CommandScenarioFor<TAggregateRoot> : IAggregateCommandGivenStateBuilder<TAggregateRoot>
        where TAggregateRoot : IAggregateRootEntity
    {
        readonly Func<IAggregateRootEntity> _sutFactory;

        public CommandScenarioFor(TAggregateRoot sut)
            : this(() => sut)
        {
        }

        public CommandScenarioFor(Func<TAggregateRoot> sutFactory)
        {
            _sutFactory = () => sutFactory();
        }

        public IAggregateCommandGivenStateBuilder<TAggregateRoot> Given(params object[] events)
        {
            if (events == null) throw new ArgumentNullException("events");
            return new AggregateCommandGivenStateBuilder<TAggregateRoot>(_sutFactory, events);
        }

        public IAggregateCommandWhenStateBuilder When(Action<TAggregateRoot> command)
        {
            if (command == null) throw new ArgumentNullException("command");
            return new AggregateCommandWhenStateBuilder(_sutFactory, new object[0],
                                                        root => command((TAggregateRoot) root));
        }
    }
}