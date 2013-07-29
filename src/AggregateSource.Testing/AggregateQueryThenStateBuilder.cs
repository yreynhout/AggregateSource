using System;

namespace AggregateSource.Testing
{
    class AggregateQueryThenStateBuilder : IAggregateQueryThenStateBuilder
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;
        readonly Func<IAggregateRootEntity, object> _when;
        readonly object _then;

        public AggregateQueryThenStateBuilder(Func<IAggregateRootEntity> sutFactory, object[] givens,
                                              Func<IAggregateRootEntity, object> when, object then)
        {
            _sutFactory = sutFactory;
            _givens = givens;
            _when = when;
            _then = then;
        }

        public EventCentricAggregateQueryTestSpecification Build()
        {
            return new EventCentricAggregateQueryTestSpecification(_sutFactory, _givens, _when, _then);
        }
    }
}