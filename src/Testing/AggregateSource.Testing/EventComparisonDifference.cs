namespace AggregateSource.Testing
{
    /// <summary>
    /// Represents a difference found between the expected and actual event.
    /// </summary>
    public class EventComparisonDifference
    {
        readonly object _expected;
        readonly object _actual;
        readonly string _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventComparisonDifference"/> class.
        /// </summary>
        /// <param name="expected">The expected event.</param>
        /// <param name="actual">The actual event.</param>
        /// <param name="message">The message describing the difference.</param>
        public EventComparisonDifference(object expected, object actual, string message)
        {
            _expected = expected;
            _actual = actual;
            _message = message;
        }

        /// <summary>
        /// Gets the expected event.
        /// </summary>
        /// <value>
        /// The expected event.
        /// </value>
        public object Expected
        {
            get { return _expected; }
        }

        /// <summary>
        /// Gets the actual event.
        /// </summary>
        /// <value>
        /// The actual event.
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