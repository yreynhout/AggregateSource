namespace AggregateSource.Testing
{
    /// <summary>
    /// Represents a difference found between the expected and actual result.
    /// </summary>
    public class ResultComparisonDifference
    {
        readonly object _expected;
        readonly object _actual;
        readonly string _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultComparisonDifference"/> class.
        /// </summary>
        /// <param name="expected">The expected result.</param>
        /// <param name="actual">The actual result.</param>
        /// <param name="message">The message describing the difference.</param>
        public ResultComparisonDifference(object expected, object actual, string message)
        {
            _expected = expected;
            _actual = actual;
            _message = message;
        }

        /// <summary>
        /// Gets the expected result.
        /// </summary>
        /// <value>
        /// The expected result.
        /// </value>
        public object Expected
        {
            get { return _expected; }
        }

        /// <summary>
        /// Gets the actual result.
        /// </summary>
        /// <value>
        /// The actual result.
        /// </value>
        public object Actual
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