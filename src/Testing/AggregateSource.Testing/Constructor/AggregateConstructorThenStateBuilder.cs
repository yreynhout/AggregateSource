using System;
using System.Linq;

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
			return new AggregateConstructorThenStateBuilder(_sutFactory, _thens.Concat(events).ToArray());
		}

		public EventCentricAggregateConstructorTestSpecification Build()
		{
			return new EventCentricAggregateConstructorTestSpecification(_sutFactory, _thens);
		}
	}
}