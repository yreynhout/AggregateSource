using System;
#if NET20
using System.Collections.Generic;
#endif
#if !NET20
using System.Linq;
#endif

namespace AggregateSource.Testing.Constructor
{
	class AggregateConstructorThenStateBuilder : IAggregateConstructorThenStateBuilder
	{
		readonly Func<IAggregateRootEntity> _sutFactory;
		readonly object[] _thens;

		public AggregateConstructorThenStateBuilder(Func<IAggregateRootEntity> sutFactory, object[] thens)
		{
			_sutFactory = sutFactory;
			_thens = thens;
		}

		public IAggregateConstructorThenStateBuilder Then(params object[] events)
		{
			if (events == null) throw new ArgumentNullException("events");
#if NET20
            var thens = new List<object>(_thens);
            thens.AddRange(events);
            return new AggregateConstructorThenStateBuilder(_sutFactory, thens.ToArray());
#else
            return new AggregateConstructorThenStateBuilder(_sutFactory, _thens.Concat(events).ToArray());
#endif
		}

		public EventCentricAggregateConstructorTestSpecification Build()
		{
			return new EventCentricAggregateConstructorTestSpecification(_sutFactory, _thens);
		}
	}
}