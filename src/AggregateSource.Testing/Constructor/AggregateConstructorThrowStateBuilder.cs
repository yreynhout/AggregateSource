using System;

namespace AggregateSource.Testing.Constructor
{
	class AggregateConstructorThrowStateBuilder : IAggregateConstructorThrowStateBuilder
	{
		readonly Func<IAggregateRootEntity> _sutFactory;
		readonly Exception _throws;

		public AggregateConstructorThrowStateBuilder(Func<IAggregateRootEntity> sutFactory, Exception throws)
		{
			_sutFactory = sutFactory;
			_throws = throws;
		}

		public ExceptionCentricAggregateConstructorTestSpecification Build()
		{
			return new ExceptionCentricAggregateConstructorTestSpecification(_sutFactory, _throws);
		}
	}
}