using System;

namespace AggregateSource.Testing.AggregateBehavior
{
    /// <summary>
    /// Represents an result-centric test specification, meaning that the outcome revolves around the result of executing a query method.
    /// </summary>
    public class ResultCentricAggregateQueryTestSpecification
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;
        readonly Func<IAggregateRootEntity, object> _when;
        readonly object _then;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultCentricAggregateQueryTestSpecification"/> class.
        /// </summary>
        /// <param name="sutFactory">The sut factory.</param>
        /// <param name="givens">The events to arrange.</param>
        /// <param name="when">The query method to act upon.</param>
        /// <param name="then">The events to assert.</param>
        public ResultCentricAggregateQueryTestSpecification(
            Func<IAggregateRootEntity> sutFactory, 
            object[] givens,
            Func<IAggregateRootEntity, object> when, 
            object then)
        {
            if (sutFactory == null) throw new ArgumentNullException("sutFactory");
            if (givens == null) throw new ArgumentNullException("givens");
            if (when == null) throw new ArgumentNullException("when");
            _sutFactory = sutFactory;
            _givens = givens;
            _when = when;
            _then = then;
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
        /// The events to arrange.
        /// </summary>
        public object[] Givens
        {
            get { return _givens; }
        }

        /// <summary>
        /// The query method to act upon.
        /// </summary>
        public Func<IAggregateRootEntity, object> When
        {
            get { return _when; }
        }

        /// <summary>
        /// The expected result to assert.
        /// </summary>
        public object Then
        {
            get { return _then; }
        }

        /// <summary>
        /// Returns a test result that indicates this specification has passed.
        /// </summary>
        /// <returns>A new <see cref="EventCentricAggregateCommandTestResult"/>.</returns>
        public ResultCentricAggregateQueryTestResult Pass()
        {
            return new ResultCentricAggregateQueryTestResult(
                this,
                TestResultState.Passed,
                Optional<object>.Empty,
                Optional<Exception>.Empty,
                Optional<object[]>.Empty);
        }

        /// <summary>
        /// Returns a test result that indicates this specification has failed because nothing was returned nor happened.
        /// </summary>
        /// <returns>A new <see cref="ResultCentricAggregateQueryTestResult"/>.</returns>
        public ResultCentricAggregateQueryTestResult Fail()
        {
            return new ResultCentricAggregateQueryTestResult(
                this,
                TestResultState.Failed,
                Optional<object>.Empty,
                Optional<Exception>.Empty,
                Optional<object[]>.Empty);
        }

        /// <summary>
        /// Returns a test result that indicates this specification has failed because different things happened.
        /// </summary>
        /// <param name="actual">The actual events</param>
        /// <returns>A new <see cref="ResultCentricAggregateQueryTestResult"/>.</returns>
        public ResultCentricAggregateQueryTestResult Fail(object[] actual)
        {
            if (actual == null) throw new ArgumentNullException("actual");
            return new ResultCentricAggregateQueryTestResult(
                this,
                TestResultState.Failed,
                Optional<object>.Empty,
                Optional<Exception>.Empty,
                new Optional<object[]>(actual));
        }

        /// <summary>
        /// Returns a test result that indicates this specification has failed because an exception happened.
        /// </summary>
        /// <param name="actual">The actual exception</param>
        /// <returns>A new <see cref="ResultCentricAggregateQueryTestResult"/>.</returns>
        public ResultCentricAggregateQueryTestResult Fail(Exception actual)
        {
            if (actual == null) throw new ArgumentNullException("actual");
            return new ResultCentricAggregateQueryTestResult(
                this,
                TestResultState.Failed,
                Optional<object>.Empty,
                new Optional<Exception>(actual),
                Optional<object[]>.Empty);
        }

        /// <summary>
        /// Returns a test result that indicates this specification has failed because a different query result was returned.
        /// </summary>
        /// <param name="actual">The actual exception</param>
        /// <returns>A new <see cref="ResultCentricAggregateQueryTestResult"/>.</returns>
        public ResultCentricAggregateQueryTestResult Fail(object actual)
        {
            return new ResultCentricAggregateQueryTestResult(
                this,
                TestResultState.Failed,
                new Optional<object>(actual),
                Optional<Exception>.Empty,
                Optional<object[]>.Empty);
        }
    }
}