using System;

namespace AggregateSource.Testing
{
    /// <summary>
    /// The result of an event centric aggregate constructor test specification run.
    /// </summary>
    public class EventCentricAggregateConstructorTestResult
    {
        readonly EventCentricAggregateConstructorTestSpecification _specification;
        readonly TestResultState _state;
        readonly Optional<object[]> _actualEvents;
        readonly Optional<Exception> _actualException;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventCentricTestResult"/> class.
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <param name="state">The state.</param>
        /// <param name="actualEvents">The actual events.</param>
        /// <param name="actualException">The actual exception.</param>
        internal EventCentricAggregateConstructorTestResult(
            EventCentricAggregateConstructorTestSpecification specification, TestResultState state,
            Optional<object[]> actualEvents, 
            Optional<Exception> actualException)
        {
            _specification = specification;
            _state = state;
            _actualEvents = actualEvents;
            _actualException = actualException;
        }

        /// <summary>
        /// Gets the test specification associated with this result.
        /// </summary>
        /// <value>
        /// The test specification.
        /// </value>
        public EventCentricAggregateConstructorTestSpecification Specification
        {
            get { return _specification; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="EventCentricTestResult"/> has passed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if passed; otherwise, <c>false</c>.
        /// </value>
        public bool Passed
        {
            get { return _state == TestResultState.Passed; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="EventCentricTestResult"/> has failed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if failed; otherwise, <c>false</c>.
        /// </value>
        public bool Failed
        {
            get { return _state == TestResultState.Failed; }
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
    }
}