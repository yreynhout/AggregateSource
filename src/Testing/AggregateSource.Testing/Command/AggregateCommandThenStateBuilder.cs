using System;
#if NET20
using System.Collections.Generic;
#endif
#if !NET20
using System.Linq;
#endif

namespace AggregateSource.Testing.Command
{
    class AggregateCommandThenStateBuilder : IAggregateCommandThenStateBuilder
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;
        readonly Action<IAggregateRootEntity> _when;
        readonly object[] _thens;

        public AggregateCommandThenStateBuilder(Func<IAggregateRootEntity> sutFactory, object[] givens,
                                                Action<IAggregateRootEntity> when, object[] thens)
        {
            _sutFactory = sutFactory;
            _givens = givens;
            _when = when;
            _thens = thens;
        }

        public IAggregateCommandThenStateBuilder Then(params object[] events)
        {
            if (events == null) throw new ArgumentNullException("events");
#if NET20
            var thens = new List<object>();
            thens.AddRange(_thens);
            thens.AddRange(events);
            return new AggregateCommandThenStateBuilder(_sutFactory, _givens, _when, thens.ToArray());
#else
            return new AggregateCommandThenStateBuilder(_sutFactory, _givens, _when, _thens.Concat(events).ToArray());
#endif
        }

        public EventCentricAggregateCommandTestSpecification Build()
        {
            return new EventCentricAggregateCommandTestSpecification(_sutFactory, _givens, _when, _thens);
        }
    }
}