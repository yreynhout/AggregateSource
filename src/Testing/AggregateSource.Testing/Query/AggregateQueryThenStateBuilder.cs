using System;

namespace AggregateSource.Testing.Query
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

        public ResultCentricAggregateQueryTestSpecification Build()
        {
            return new ResultCentricAggregateQueryTestSpecification(_sutFactory, _givens, _when, _then);
        }
    }
}