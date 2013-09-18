using System;
using System.Linq;

namespace AggregateSource.Testing.Command
{
    class AggregateCommandThenStateBuilder : IAggregateCommandThenStateBuilder
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;
        readonly Action<IAggregateRootEntity> _when;
        readonly object[] _thens;

        public AggregateCommandThenStateBuilder(Func<IAggregateRootEntity> sutFactory, object[] givens,
                                                Action<IAggregateRootEntity> when, object[] thens)
        {
            _sutFactory = sutFactory;
            _givens = givens;
            _when = when;
            _thens = thens;
        }

        public IAggregateCommandThenStateBuilder Then(params object[] events)
        {
            if (events == null) throw new ArgumentNullException("events");
            return new AggregateCommandThenStateBuilder(_sutFactory, _givens, _when, _thens.Concat(events).ToArray());
        }

        public EventCentricAggregateCommandTestSpecification Build()
        {
            return new EventCentricAggregateCommandTestSpecification(_sutFactory, _givens, _when, _thens);
        }
    }
}