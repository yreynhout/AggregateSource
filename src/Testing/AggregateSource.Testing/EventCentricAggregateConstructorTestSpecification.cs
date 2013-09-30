using System;

namespace AggregateSource.Testing
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
		    if (sutFactory == null) throw new ArgumentNullException("sutFactory");
		    if (thens == null) throw new ArgumentNullException("thens");
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

        /// <summary>
        /// Returns a test result that indicates this specification has passed.
        /// </summary>
        /// <returns>A new <see cref="EventCentricAggregateConstructorTestResult"/>.</returns>
        public EventCentricAggregateConstructorTestResult Pass()
        {
            return new EventCentricAggregateConstructorTestResult(
                this,
                TestResultState.Passed,
                Optional<object[]>.Empty,
                Optional<Exception>.Empty);
        }

        /// <summary>
        /// Returns a test result that indicates this specification has failed because different things happened.
        /// </summary>
        /// <param name="actual">The actual events</param>
        /// <returns>A new <see cref="EventCentricAggregateConstructorTestResult"/>.</returns>
        public EventCentricAggregateConstructorTestResult Fail(object[] actual)
        {
            if (actual == null) throw new ArgumentNullException("actual");
            return new EventCentricAggregateConstructorTestResult(
                this,
                TestResultState.Failed,
                new Optional<object[]>(actual),
                Optional<Exception>.Empty);
        }

        /// <summary>
        /// Returns a test result that indicates this specification has failed because an exception happened.
        /// </summary>
        /// <param name="actual">The actual exception</param>
        /// <returns>A new <see cref="EventCentricAggregateConstructorTestResult"/>.</returns>
        public EventCentricAggregateConstructorTestResult Fail(Exception actual)
        {
            if (actual == null) throw new ArgumentNullException("actual");
            return new EventCentricAggregateConstructorTestResult(
                this,
                TestResultState.Failed,
                Optional<object[]>.Empty,
                new Optional<Exception>(actual));
        }
	}
}