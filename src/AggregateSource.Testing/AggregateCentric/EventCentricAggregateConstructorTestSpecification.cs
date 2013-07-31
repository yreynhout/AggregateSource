using System;

namespace AggregateSource.Testing.AggregateCentric
{
	/// <summary>
	/// Represents an event centric test specification, meaning that the outcome revolves around events as the result of constructing an aggregate.
	/// </summary>
	public class EventCentricAggregateConstructorTestSpecification
	{
		readonly Func<IAggregateRootEntity> _sutFactory;
		readonly object[] _thens;

		/// <summary>
		/// Initializes a new instance of the <see cref="EventCentricAggregateConstructorTestSpecification"/> class.
		/// </summary>
		/// <param name="sutFactory">The sut factory.</param>
		/// <param name="thens">The events to assert.</param>
		public EventCentricAggregateConstructorTestSpecification(Func<IAggregateRootEntity> sutFactory, object[] thens)
		{
			_sutFactory = sutFactory;
			_thens = thens;
		}

		/// <summary>
		/// Gets the sut factory.
		/// </summary>
		/// <value>
		/// The sut factory.
		/// </value>
		public Func<IAggregateRootEntity> SutFactory
		{
			get { return _sutFactory; }
		}

		/// <summary>
		/// The expected events to assert.
		/// </summary>
		public object[] Thens
		{
			get { return _thens; }
		}
	}
}