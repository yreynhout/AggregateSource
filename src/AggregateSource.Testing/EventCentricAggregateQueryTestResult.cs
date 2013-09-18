using System;

namespace AggregateSource.Testing
{
    /// <summary>
    /// The result of a result centric aggregate query test specification run.
    /// </summary>
    public class ResultCentricAggregateQueryTestResult
    {
        readonly ResultCentricAggregateQueryTestSpecification _specification;
        readonly TestResultState _state;
        readonly Optional<object[]> _actualEvents;
        readonly Optional<object> _actualResult;
        readonly Optional<Exception> _actualException;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultCentricAggregateQueryTestResult"/> class.
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <param name="state">The state.</param>
        /// <param name="actualResult">The actual result.</param>
        /// <param name="actualException">The actual exception.</param>
        /// <param name="actualEvents">The actual events.</param>
        internal ResultCentricAggregateQueryTestResult(ResultCentricAggregateQueryTestSpecification specification, TestResultState state, Optional<object> actualResult, Optional<Exception> actualException, Optional<object[]> actualEvents)
        {
            _specification = specification;
            _state = state;
            _actualResult = actualResult;
            _actualException = actualException;
            _actualEvents = actualEvents;
        }

        /// <summary>
        /// Gets the test specification associated with this result.
        /// </summary>
        /// <value>
        /// The test specification.
        /// </value>
        public ResultCentricAggregateQueryTestSpecification Specification
        {
            get { return _specification; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ResultCentricAggregateQueryTestResult"/> has passed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if passed; otherwise, <c>false</c>.
        /// </value>
        public bool Passed
        {
            get { return _state == TestResultState.Passed; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ResultCentricAggregateQueryTestResult"/> has failed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if failed; otherwise, <c>false</c>.
        /// </value>
        public bool Failed
        {
            get { return _state == TestResultState.Failed; }
        }

        /// <summary>
        /// Gets the result that happened instead of the expected one, or empty if passed.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public Optional<object> ButResult
        {
            get { return _actualResult; }
        }

        /// <summary>
        /// Gets the exception that happened instead of the expected events, or empty if passed.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Optional<Exception> ButException
        {
            get { return _actualException; }
        }

        /// <summary>
        /// Gets the events that happened instead of the expected ones, or empty if passed.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        public Optional<object[]> ButEvents
        {
            get { return _actualEvents; }
        }
    }
}