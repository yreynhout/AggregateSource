using System;

namespace AggregateSource.Testing
{
    /// <summary>
    /// Represents an exception centric test specification, meaning that the outcome revolves around an exception.
    /// </summary>
    public class ExceptionCentricTestSpecification
    {
        private readonly Tuple<string, object>[] _givens;
        private readonly object _when;
        private readonly Exception _throws;

        /// <summary>
        /// Initializes a new <see cref="ExceptionCentricTestSpecification"/> instance.
        /// </summary>
        /// <param name="givens">The specification givens.</param>
        /// <param name="when">The specification when.</param>
        /// <param name="throws">The specification exception thrown.</param>
        public ExceptionCentricTestSpecification(Tuple<string, object>[] givens, object when, Exception throws)
        {
            if (givens == null) throw new ArgumentNullException("givens");
            if (when == null) throw new ArgumentNullException("when");
            if (throws == null) throw new ArgumentNullException("throws");
            _givens = givens;
            _when = when;
            _throws = throws;
        }

        /// <summary>
        /// The events to arrange.
        /// </summary>
        public Tuple<string, object>[] Givens
        {
            get { return _givens; }
        }

        /// <summary>
        /// The message to act upon.
        /// </summary>
        public object When
        {
            get { return _when; }
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
        /// <returns>A new <see cref="ExceptionCentricTestResult"/>.</returns>
        public ExceptionCentricTestResult Pass()
        {
            return new ExceptionCentricTestResult(this, TestResultState.Passed);
        }

        /// <summary>
        /// Returns a test result that indicates this specification has failed.
        /// </summary>
        /// <returns>A new <see cref="ExceptionCentricTestResult"/>.</returns>
        public ExceptionCentricTestResult Fail()
        {
            return new ExceptionCentricTestResult(this, TestResultState.Failed);
        }

        /// <summary>
        /// Returns a test result that indicates this specification has failed.
        /// </summary>
        /// <param name="actual">The actual exception thrown</param>
        /// <returns>A new <see cref="ExceptionCentricTestResult"/>.</returns>
        public ExceptionCentricTestResult Fail(Exception actual)
        {
            if (actual == null) throw new ArgumentNullException("actual");
            return new ExceptionCentricTestResult(this, TestResultState.Failed, actualException: actual);
        }

        /// <summary>
        /// Returns a test result that indicates this specification has failed because different things happened.
        /// </summary>
        /// <param name="actual">The actual events</param>
        /// <returns>A new <see cref="ExceptionCentricTestResult"/>.</returns>
        public ExceptionCentricTestResult Fail(Tuple<string, object>[] actual)
        {
            if (actual == null) throw new ArgumentNullException("actual");
            return new ExceptionCentricTestResult(this, TestResultState.Failed, actualEvents: actual);
        }

        /// <summary>
        /// Determines whether the specified <see cref="ExceptionCentricTestSpecification" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="ExceptionCentricTestSpecification" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="ExceptionCentricTestSpecification" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        protected bool Equals(ExceptionCentricTestSpecification other)
        {
            return
                Equals(_givens, other._givens) &&
                Equals(_when, other._when) &&
                Equals(_throws, other._throws);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ExceptionCentricTestSpecification) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return
                _givens.GetHashCode() ^
                _when.GetHashCode() ^
                _throws.GetHashCode();
        }
    }
}