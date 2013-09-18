using System;

namespace AggregateSource.Testing.Query
{
    class AggregateQueryWhenStateBuilder<TResult> : IAggregateQueryWhenStateBuilder<TResult>
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;
        readonly Func<IAggregateRootEntity, object> _when;

        public AggregateQueryWhenStateBuilder(Func<IAggregateRootEntity> sutFactory, object[] givens,
                                              Func<IAggregateRootEntity, object> when)
        {
            _sutFactory = sutFactory;
            _givens = givens;
            _when = when;
        }

        public IAggregateQueryThenStateBuilder Then(TResult result)
        {
            return new AggregateQueryThenStateBuilder(_sutFactory, _givens, _when, result);
        }

        public IAggregateQueryThrowStateBuilder Throws(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException("exception");
            return new AggregateQueryThrowStateBuilder(_sutFactory, _givens, _when, exception);
        }
    }
}