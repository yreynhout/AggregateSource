using System;

namespace AggregateSource.Testing.Command
{
    class AggregateCommandThrowStateBuilder : IAggregateCommandThrowStateBuilder
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;
        readonly Action<IAggregateRootEntity> _when;
        readonly Exception _throws;

        public AggregateCommandThrowStateBuilder(Func<IAggregateRootEntity> sutFactory, object[] givens,
                                                 Action<IAggregateRootEntity> when, Exception throws)
        {
            _sutFactory = sutFactory;
            _givens = givens;
            _when = when;
            _throws = throws;
        }

        public ExceptionCentricAggregateCommandTestSpecification Build()
        {
            return new ExceptionCentricAggregateCommandTestSpecification(_sutFactory, _givens, _when, _throws);
        }
    }
}