using System;

namespace AggregateSource.Testing.AggregateBehavior.Query
{
    class AggregateQueryGivenNoneStateBuilder<TAggregateRoot> : IAggregateQueryGivenNoneStateBuilder<TAggregateRoot> where TAggregateRoot : IAggregateRootEntity
    {
        readonly Func<IAggregateRootEntity> _sutFactory;

        public AggregateQueryGivenNoneStateBuilder(Func<IAggregateRootEntity> sutFactory)
        {
            _sutFactory = sutFactory;
        }

        public IAggregateQueryWhenStateBuilder<TResult> When<TResult>(Func<TAggregateRoot, TResult> query)
        {
            return new AggregateQueryWhenStateBuilder<TResult>(_sutFactory, new object[0], root => query((TAggregateRoot)root));
        }
    }
}