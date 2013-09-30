using System;

namespace AggregateSource.Testing
{
	/// <summary>
	/// Represents an exception centric constructor test specification, meaning that the outcome revolves around an exception as a result of constructing an aggregate.
	/// </summary>
	public class ExceptionCentricAggregateConstructorTestSpecification
	{
		readonly Func<IAggregateRootEntity> _sutFactory;
		readonly Exception _throws;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExceptionCentricAggregateConstructorTestSpecification"/> class.
		/// </summary>
		/// <param name="sutFactory">The sut factory.</param>
		/// <param name="throws">The expected exception to assert.</param>
		public ExceptionCentricAggregateConstructorTestSpecification(Func<IAggregateRootEntity> sutFactory, Exception throws)
		{
		    if (sutFactory == null) throw new ArgumentNullException("sutFactory");
		    if (throws == null) throw new ArgumentNullException("throws");
		    _sutFactory = sutFactory;
			_throws = throws;
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
		/// The expected exception to assert.
		/// </summary>
		public Exception Throws
		{
			get { return _throws; }
		}

        /// <summary>
        /// Returns a test result that indicates this specification has passed.
        /// </summary>
        /// <returns>A new <see cref="ExceptionCentricAggregateConstructorTestResult"/>.</returns>
        public ExceptionCentricAggregateConstructorTestResult Pass()
        {
            return new ExceptionCentricAggregateConstructorTestResult(
                this,
                TestResultState.Passed,
                Optional<Exception>.Empty,
                Optional<object[]>.Empty);
        }

        /// <summary>
        /// Returns a test result that indicates this specification has failed because nothing happened.
        /// </summary>
        /// <returns>A new <see cref="ExceptionCentricAggregateConstructorTestResult"/>.</returns>
        public ExceptionCentricAggregateConstructorTestResult Fail()
        {
            return new ExceptionCentricAggregateConstructorTestResult(
                this,
                TestResultState.Failed,
                Optional<Exception>.Empty,
                Optional<object[]>.Empty);
        }

        /// <summary>
        /// Returns a test result that indicates this specification has failed because different things happened.
        /// </summary>
        /// <param name="actual">The actual events</param>
        /// <returns>A new <see cref="ExceptionCentricAggregateConstructorTestResult"/>.</returns>
        public ExceptionCentricAggregateConstructorTestResult Fail(object[] actual)
        {
            if (actual == null) throw new ArgumentNullException("actual");
            return new ExceptionCentricAggregateConstructorTestResult(
                this,
                TestResultState.Failed,
                Optional<Exception>.Empty,
                new Optional<object[]>(actual));
        }

        /// <summary>
        /// Returns a test result that indicates this specification has failed because an exception happened.
        /// </summary>
        /// <param name="actual">The actual exception</param>
        /// <returns>A new <see cref="ExceptionCentricAggregateConstructorTestResult"/>.</returns>
        public ExceptionCentricAggregateConstructorTestResult Fail(Exception actual)
        {
            if (actual == null) throw new ArgumentNullException("actual");
            return new ExceptionCentricAggregateConstructorTestResult(
                this,
                TestResultState.Failed,
                new Optional<Exception>(actual),
                Optional<object[]>.Empty);
        }
	}
}