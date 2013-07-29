using System;

namespace AggregateSource.Testing
{
    /// <summary>
    /// Represents syntactic sugar to compose facts for use in the testing API.
    /// </summary>
    public class BatchOf
    {
        static readonly Tuple<string, object>[] Empty = new Tuple<string, object>[0];

        readonly Tuple<string, object>[] _facts;

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchOf"/> class.
        /// </summary>
        public BatchOf()
        {
            _facts = Empty;
        }

        BatchOf(Tuple<string, object>[] facts)
        {
            _facts = facts;
        }

        /// <summary>
        /// Defines a set of facts that happened to a particular aggregate.
        /// </summary>
        /// <param name="identifier">The aggregate identifier the events apply to.</param>
        /// <param name="events">The events that occurred.</param>
        /// <returns>A batch of facts.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="identifier"/> or <paramref name="events"/> is <c>null</c>.</exception>
        public BatchOf Fact(string identifier, params object[] events)
        {
            if (identifier == null) throw new ArgumentNullException("identifier");
            if (events == null) throw new ArgumentNullException("events");
            if (events.Length == 0) return this;
            var combinedFacts = new Tuple<string, object>[_facts.Length + events.Length];
            _facts.CopyTo(combinedFacts, 0);
            for (var index = 0; index < events.Length; index++)
            {
                combinedFacts[_facts.Length + index] = new Tuple<string, object>(identifier, events[index]);
            }
            return new BatchOf(combinedFacts);
        }

        /// <summary>
        /// Defines a set of facts that happened.
        /// </summary>
        /// <param name="facts">The facts that occurred.</param>
        /// <returns>A batch of facts.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="facts"/> is <c>null</c>.</exception>
        public BatchOf Facts(params Tuple<string, object>[] facts)
        {
            if (facts == null) throw new ArgumentNullException("facts");
            var combinedFacts = new Tuple<string, object>[_facts.Length + facts.Length];
            _facts.CopyTo(combinedFacts, 0);
            facts.CopyTo(combinedFacts, _facts.Length);
            return new BatchOf(combinedFacts);
        }

        /// <summary>
        /// Implicitly converts a batch of facts into a testing API friendly array.
        /// </summary>
        /// <param name="batch">The batch of facts.</param>
        /// <returns>An array of facts.</returns>
        public static implicit operator Tuple<string, object>[](BatchOf batch)
        {
            return batch._facts;
        }
    }
}