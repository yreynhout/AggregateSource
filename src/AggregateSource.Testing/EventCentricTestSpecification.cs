using System;

namespace AggregateSource.Testing
{
    /// <summary>
    /// Represents an event centric test specification, meaning that the outcome revolves around events.
    /// </summary>
    public class EventCentricTestSpecification
    {
        private readonly Tuple<string, object>[] _givens;
        private readonly object _when;
        private readonly Tuple<string, object>[] _thens;

        /// <summary>
        /// Initializes a new <see cref="EventCentricTestSpecification"/> instance.
        /// </summary>
        /// <param name="givens">The specification givens.</param>
        /// <param name="when">The specification when.</param>
        /// <param name="thens">The specification thens.</param>
        public EventCentricTestSpecification(Tuple<string, object>[] givens, object when, Tuple<string, object>[] thens)
        {
            if (givens == null) throw new ArgumentNullException("givens");
            if (when == null) throw new ArgumentNullException("when");
            if (thens == null) throw new ArgumentNullException("thens");
            _givens = givens;
            _when = when;
            _thens = thens;
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
        /// The expected events to assert.
        /// </summary>
        public Tuple<string, object>[] Thens
        {
            get { return _thens; }
        }

        /// <summary>
        /// Returns a test result that indicates this specification has passed.
        /// </summary>
        /// <returns>A new <see cref="EventCentricTestResult"/>.</returns>
        public EventCentricTestResult Pass()
        {
            return new EventCentricTestResult(this, TestResultState.Passed);
        }

        /// <summary>
        /// Returns a test result that indicates this specification has failed because nothing happened.
        /// </summary>
        /// <returns>A new <see cref="EventCentricTestResult"/>.</returns>
        public EventCentricTestResult Fail()
        {
            return new EventCentricTestResult(this, TestResultState.Failed);
        }

        /// <summary>
        /// Returns a test result that indicates this specification has failed because different things happened.
        /// </summary>
        /// <param name="actual">The actual events</param>
        /// <returns>A new <see cref="EventCentricTestResult"/>.</returns>
        public EventCentricTestResult Fail(Tuple<string, object>[] actual)
        {
            if (actual == null) throw new ArgumentNullException("actual");
            return new EventCentricTestResult(this, TestResultState.Failed, actualEvents: actual);
        }

        /// <summary>
        /// Returns a test result that indicates this specification has failed because an exception happened.
        /// </summary>
        /// <param name="actual">The actual exception</param>
        /// <returns>A new <see cref="EventCentricTestResult"/>.</returns>
        public EventCentricTestResult Fail(Exception actual)
        {
            if (actual == null) throw new ArgumentNullException("actual");
            return new EventCentricTestResult(this, TestResultState.Failed, actualException: actual);
        }

        /// <summary>
        /// Determines whether the specified <see cref="EventCentricTestSpecification" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="EventCentricTestSpecification" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="EventCentricTestSpecification" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        protected bool Equals(EventCentricTestSpecification other)
        {
            return
                Equals(_givens, other._givens) &&
                Equals(_when, other._when) &&
                Equals(_thens, other._thens);
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
            return Equals((EventCentricTestSpecification) obj);
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
                _thens.GetHashCode();
        }
    }
}