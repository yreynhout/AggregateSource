using System;
#if NET20
using System.Collections.Generic;
#endif
#if !NET20
using System.Linq;
#endif

namespace AggregateSource.Testing.Factory
{
    class AggregateFactoryThenStateBuilder : IAggregateFactoryThenStateBuilder
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;
        readonly Func<IAggregateRootEntity, IAggregateRootEntity> _when;
        readonly object[] _thens;

        public AggregateFactoryThenStateBuilder(Func<IAggregateRootEntity> sutFactory, object[] givens,
                                                Func<IAggregateRootEntity, IAggregateRootEntity> when, object[] thens)
        {
            _sutFactory = sutFactory;
            _givens = givens;
            _when = when;
            _thens = thens;
        }

        public IAggregateFactoryThenStateBuilder Then(params object[] events)
        {
            if (events == null) throw new ArgumentNullException("events");
#if NET20
            var thens = new List<object>(_thens);
            thens.AddRange(events);
            return new AggregateFactoryThenStateBuilder(_sutFactory, _givens, _when, thens.ToArray());
#else
            return new AggregateFactoryThenStateBuilder(_sutFactory, _givens, _when, _thens.Concat(events).ToArray());
#endif
        }

        public EventCentricAggregateFactoryTestSpecification Build()
        {
            return new EventCentricAggregateFactoryTestSpecification(_sutFactory, _givens, _when, _thens);
        }
    }
}