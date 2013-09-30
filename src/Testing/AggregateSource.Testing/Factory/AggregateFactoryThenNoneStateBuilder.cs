using System;

namespace AggregateSource.Testing.Factory
{
    class AggregateFactoryThenNoneStateBuilder : IAggregateFactoryThenNoneStateBuilder
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;
        readonly Func<IAggregateRootEntity, IAggregateRootEntity> _when;

        public AggregateFactoryThenNoneStateBuilder(Func<IAggregateRootEntity> sutFactory, object[] givens,
            Func<IAggregateRootEntity, IAggregateRootEntity> when)
        {
            _sutFactory = sutFactory;
            _givens = givens;
            _when = when;
        }

        
        public EventCentricAggregateFactoryTestSpecification Build()
        {
            return new EventCentricAggregateFactoryTestSpecification(_sutFactory, _givens, _when, new object[0]);
        }
    }
}