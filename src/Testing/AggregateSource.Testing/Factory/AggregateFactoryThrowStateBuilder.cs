using System;

namespace AggregateSource.Testing.Factory
{
    class AggregateFactoryThrowStateBuilder : IAggregateFactoryThrowStateBuilder
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;
        readonly Func<IAggregateRootEntity, IAggregateRootEntity> _when;
        readonly Exception _throws;

        public AggregateFactoryThrowStateBuilder(Func<IAggregateRootEntity> sutFactory, object[] givens,
                                                 Func<IAggregateRootEntity, IAggregateRootEntity> when, Exception throws)
        {
            _sutFactory = sutFactory;
            _givens = givens;
            _when = when;
            _throws = throws;
        }

        public ExceptionCentricAggregateFactoryTestSpecification Build()
        {
            return new ExceptionCentricAggregateFactoryTestSpecification(_sutFactory, _givens, _when, _throws);
        }
    }
}