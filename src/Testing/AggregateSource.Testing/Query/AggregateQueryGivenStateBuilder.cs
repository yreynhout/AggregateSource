using System;
#if NET20
using System.Collections.Generic;
#endif
#if !NET20
using System.Linq;
#endif

namespace AggregateSource.Testing.Query
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
#if NET20
            var givens = new List<object>(_givens);
            givens.AddRange(events);
            return new AggregateQueryGivenStateBuilder<TAggregateRoot>(_sutFactory, givens.ToArray());
#else
            return new AggregateQueryGivenStateBuilder<TAggregateRoot>(_sutFactory, _givens.Concat(events).ToArray());
#endif
        }

        public IAggregateQueryWhenStateBuilder<TResult> When<TResult>(Func<TAggregateRoot, TResult> query)
        {
            if (query == null) throw new ArgumentNullException("query");
            return new AggregateQueryWhenStateBuilder<TResult>(_sutFactory, _givens,
                                                               root => query((TAggregateRoot) root));
        }
    }
}