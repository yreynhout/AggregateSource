using System;
using System.Linq;

namespace AggregateSource.Testing.AggregateBehavior.Query
{
    class AggregateQueryGivenStateBuilder<TAggregateRoot> : IAggregateQueryGivenStateBuilder<TAggregateRoot>
        where TAggregateRoot : IAggregateRootEntity
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;

        public AggregateQueryGivenStateBuilder(Func<IAggregateRootEntity> sutFactory, object[] givens)
        {
            _sutFactory = sutFactory;
            _givens = givens;
        }

        public IAggregateQueryGivenStateBuilder<TAggregateRoot> Given(params object[] events)
        {
            if (events == null) throw new ArgumentNullException("events");
            return new AggregateQueryGivenStateBuilder<TAggregateRoot>(_sutFactory, _givens.Concat(events).ToArray());
        }

        public IAggregateQueryWhenStateBuilder<TResult> When<TResult>(Func<TAggregateRoot, TResult> query)
        {
            if (query == null) throw new ArgumentNullException("query");
            return new AggregateQueryWhenStateBuilder<TResult>(_sutFactory, _givens,
                                                               root => query((TAggregateRoot) root));
        }
    }
}