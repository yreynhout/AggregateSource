using System;

namespace AggregateSource.Testing
{
    /// <summary>
    /// Represents a difference found between the expected and actual exception.
    /// </summary>
    public class ExceptionComparisonDifference
    {
        readonly Exception _expected;
        readonly Exception _actual;
        readonly string _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionComparisonDifference"/> class.
        /// </summary>
        /// <param name="expected">The expected exception.</param>
        /// <param name="actual">The actual exception.</param>
        /// <param name="message">The message describing the difference.</param>
        public ExceptionComparisonDifference(Exception expected, Exception actual, string message)
        {
            _expected = expected;
            _actual = actual;
            _message = message;
        }

        /// <summary>
        /// Gets the expected exception.
        /// </summary>
        /// <value>
        /// The expected exception.
        /// </value>
        public Exception Expected
        {
            get { return _expected; }
        }

        /// <summary>
        /// Gets the actual exception.
        /// </summary>
        /// <value>
        /// The actual exception.
        /// </value>
        public Exception Actual
        {
            get { return _actual; }
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message
        {
            get { return _message; }
        }
    }
}