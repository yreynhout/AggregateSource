using System;

namespace AggregateSource.Testing
{
    internal class AggregateQueryThrowStateBuilder : IAggregateQueryThrowStateBuilder
    {
        private readonly Func<IAggregateRootEntity> _sutFactory;
        private readonly object[] _givens;
        private readonly Func<IAggregateRootEntity, object> _when;
        private readonly Exception _throws;

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