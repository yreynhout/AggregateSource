using System;
using AggregateSource.Testing.AggregateBehavior.Constructor;

namespace AggregateSource.Testing.AggregateBehavior
{
	/// <summary>
	/// A when-then test specification bootstrapper for testing an aggregate constructor,
	/// i.e. a method on the aggregate that returns an instance of the aggregate.
	/// </summary>
    public class ConstructorScenarioFor<TAggregateRoot> : IAggregateConstructorWhenStateBuilder
        where TAggregateRoot : IAggregateRootEntity
	{
		private readonly Func<IAggregateRootEntity> _constructor;

		/// <summary>
		/// Creates a new constructor scenario testing the constructor passed as a delegate.
		/// </summary>
		/// <param name="constructor">The constructor invocation.</param>
		/// <exception cref="ArgumentNullException">If the constructor delegate is null.</exception>
        public ConstructorScenarioFor(Func<TAggregateRoot> constructor)
		{
			if (constructor == null)
				throw new ArgumentNullException("constructor");

			_constructor = () => constructor();
		}

		/// <summary>
		/// Then events should have occurred.
		/// </summary>
		/// <param name="events">The events that should have occurred.</param>
		/// <returns>A builder continuation.</returns>
		public IAggregateConstructorThenStateBuilder Then(params object[] events)
		{
			if (events == null) throw new ArgumentNullException("events");
			return new AggregateConstructorThenStateBuilder(_constructor, events);
		}

		/// <summary>
		/// Throws an exception.
		/// </summary>
		/// <param name="exception">The exception thrown.</param>
		/// <returns>A builder continuation.</returns>
		public IAggregateConstructorThrowStateBuilder Throws(Exception exception)
		{
			if (exception == null) throw new ArgumentNullException("exception");
			return new AggregateConstructorThrowStateBuilder(_constructor, exception);
		}
	}
}