using System;
#if NET20
using System.Collections.Generic;
#endif
#if !NET20
using System.Linq;
#endif

namespace AggregateSource.Testing.Factory
{
    class AggregateFactoryGivenStateBuilder<TAggregateRoot> : IAggregateFactoryGivenStateBuilder<TAggregateRoot>
        where TAggregateRoot : IAggregateRootEntity
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;

        public AggregateFactoryGivenStateBuilder(Func<IAggregateRootEntity> sutFactory, object[] givens)
        {
            _sutFactory = sutFactory;
            _givens = givens;
        }

        public IAggregateFactoryGivenStateBuilder<TAggregateRoot> Given(params object[] events)
        {
            if (events == null) throw new ArgumentNullException("events");
#if NET20
            var givens = new List<object>(_givens);
            givens.AddRange(events);
            return new AggregateFactoryGivenStateBuilder<TAggregateRoot>(_sutFactory, givens.ToArray());
#else
            return new AggregateFactoryGivenStateBuilder<TAggregateRoot>(_sutFactory, _givens.Concat(events).ToArray());
#endif
        }

        public IAggregateFactoryWhenStateBuilder When<TAggregateRootResult>(
            Func<TAggregateRoot, TAggregateRootResult> factory) where TAggregateRootResult : IAggregateRootEntity
        {
            if (factory == null) throw new ArgumentNullException("factory");
            return new AggregateFactoryWhenStateBuilder(_sutFactory, _givens, root => factory((TAggregateRoot) root));
        }
    }
}