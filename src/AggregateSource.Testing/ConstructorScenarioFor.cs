using System;

namespace AggregateSource.Testing
{
	/// <summary>
	/// A when-then test specification bootstrapper for testing an aggregate constructor,
	/// i.e. a method on the aggregate that returns an instance of the aggregate.
	/// </summary>
	public class ConstructorScenarioFor : IAggregateConstructorWhenStateBuilder
	{
		private readonly Func<IAggregateRootEntity> _ctor;

		/// <summary>
		/// Creates a new constructor scenario testing the constructor passed as a delegate.
		/// </summary>
		/// <param name="ctor">The constructor delegate.</param>
		/// <exception cref="ArgumentNullException">If the constructor delegate is null.</exception>
		public ConstructorScenarioFor(Func<IAggregateRootEntity> ctor)
		{
			if (ctor == null)
				throw new ArgumentNullException("ctor");

			_ctor = ctor;
		}

		/// <summary>
		/// Then events should have occurred.
		/// </summary>
		/// <param name="events">The events that should have occurred.</param>
		/// <returns>A builder continuation.</returns>
		public IAggregateConstructorThenStateBuilder Then(params object[] events)
		{
			if (events == null) throw new ArgumentNullException("events");
			return new AggregateConstructorThenStateBuilder(_ctor, events);
		}

		/// <summary>
		/// Throws an exception.
		/// </summary>
		/// <param name="exception">The exception thrown.</param>
		/// <returns>A builder continuation.</returns>
		public IAggregateConstructorThrowStateBuilder Throws(Exception exception)
		{
			if (exception == null) throw new ArgumentNullException("exception");
			return new AggregateConstructorThrowStateBuilder(_ctor, exception);
		}
	}
}