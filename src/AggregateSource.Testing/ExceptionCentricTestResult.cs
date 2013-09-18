using System;

namespace AggregateSource.Testing
{
    /// <summary>
    /// The result of an exception centric test specification.
    /// </summary>
    public class ExceptionCentricTestResult
    {
        readonly ExceptionCentricTestSpecification _specification;
        readonly TestResultState _state;
        readonly Optional<Exception> _actualException;
        readonly Optional<Fact[]> _actualEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionCentricTestResult"/> class.
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <param name="state">The state.</param>
        /// <param name="actualException">The actual exception.</param>
        /// <param name="actualEvents">The actual events.</param>
        internal ExceptionCentricTestResult(ExceptionCentricTestSpecification specification, TestResultState state,
                                            Optional<Exception> actualException,
                                            Optional<Fact[]> actualEvents)
        {
            _specification = specification;
            _state = state;
            _actualException = actualException;
            _actualEvents = actualEvents;
        }

        /// <summary>
        /// Gets the test specification associated with this result.
        /// </summary>
        /// <value>
        /// The test specification.
        /// </value>
        public ExceptionCentricTestSpecification Specification
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
        /// Gets the exception that happened instead of the expected one, or empty if one didn't happen at all.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Optional<Exception> ButException
        {
            get { return _actualException; }
        }

        /// <summary>
        /// Gets the events that happened instead of the expected exception, or empty if none happened at all.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        public Optional<Fact[]> ButEvents
        {
            get { return _actualEvents; }
        }
    }
}