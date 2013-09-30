using System;

namespace AggregateSource.Testing.Query
{
    class AggregateQueryThrowStateBuilder : IAggregateQueryThrowStateBuilder
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;
        readonly Func<IAggregateRootEntity, object> _when;
        readonly Exception _throws;

        public AggregateQueryThrowStateBuilder(Func<IAggregateRootEntity> sutFactory, object[] givens,
                                               Func<IAggregateRootEntity, object> when, Exception throws)
        {
            _sutFactory = sutFactory;
            _givens = givens;
            _when = when;
            _throws = throws;
        }

        public ExceptionCentricAggregateQueryTestSpecification Build()
        {
            return new ExceptionCentricAggregateQueryTestSpecification(_sutFactory, _givens, _when, _throws);
        }
    }
}