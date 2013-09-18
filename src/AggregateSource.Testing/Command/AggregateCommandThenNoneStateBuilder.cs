using System;

namespace AggregateSource.Testing.Command
{
    class AggregateCommandThenNoneStateBuilder : IAggregateCommandThenNoneStateBuilder
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;
        readonly Action<IAggregateRootEntity> _when;

        public AggregateCommandThenNoneStateBuilder(Func<IAggregateRootEntity> sutFactory, object[] givens, Action<IAggregateRootEntity> when)
        {
            _sutFactory = sutFactory;
            _givens = givens;
            _when = when;
        }

        public EventCentricAggregateCommandTestSpecification Build()
        {
            return new EventCentricAggregateCommandTestSpecification(_sutFactory, _givens, _when, new object[0]);
        }
    }
}