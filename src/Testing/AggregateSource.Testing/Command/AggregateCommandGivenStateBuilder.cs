using System;
using System.Linq;

namespace AggregateSource.Testing.Command
{
    class AggregateCommandGivenStateBuilder<TAggregateRoot> : IAggregateCommandGivenStateBuilder<TAggregateRoot>
        where TAggregateRoot : IAggregateRootEntity
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;

        public AggregateCommandGivenStateBuilder(Func<IAggregateRootEntity> sutFactory, object[] givens)
        {
            _sutFactory = sutFactory;
            _givens = givens;
        }

        public IAggregateCommandGivenStateBuilder<TAggregateRoot> Given(params object[] events)
        {
            if (events == null) throw new ArgumentNullException("events");
            return new AggregateCommandGivenStateBuilder<TAggregateRoot>(_sutFactory, _givens.Concat(events).ToArray());
        }

        public IAggregateCommandWhenStateBuilder When(Action<TAggregateRoot> command)
        {
            if (command == null) throw new ArgumentNullException("command");
            return new AggregateCommandWhenStateBuilder(_sutFactory, _givens, root => command((TAggregateRoot) root));
        }
    }
}