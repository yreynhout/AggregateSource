namespace AggregateSource.Testing
{
    /// <summary>
    /// Represents a difference found between the expected and actual fact.
    /// </summary>
    public class FactComparisonDifference
    {
        readonly Fact _expected;
        readonly Fact _actual;
        readonly string _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="FactComparisonDifference"/> class.
        /// </summary>
        /// <param name="expected">The expected fact.</param>
        /// <param name="actual">The actual fact.</param>
        /// <param name="message">The message describing the difference.</param>
        public FactComparisonDifference(Fact expected, Fact actual, string message)
        {
            _expected = expected;
            _actual = actual;
            _message = message;
        }

        /// <summary>
        /// Gets the expected fact.
        /// </summary>
        /// <value>
        /// The expected fact.
        /// </value>
        public Fact Expected
        {
            get { return _expected; }
        }

        /// <summary>
        /// Gets the actual fact.
        /// </summary>
        /// <value>
        /// The actual fact.
        /// </value>
        public Fact Actual
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