using System;

namespace StreamSource
{
    public class EventStreamChangeset
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamChangeset"/> class.
        /// </summary>
        /// <param name="causationId">The causation id.</param>
        /// <param name="correlationId">The correlation id.</param>
        /// <param name="changes">The changes.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="changes"/> is null.</exception>
        public EventStreamChangeset(Guid causationId, Guid correlationId, EventStreamChange[] changes)
        {
            if (changes == null) throw new ArgumentNullException("changes");
            CausationId = causationId;
            CorrelationId = correlationId;
            Changes = changes;
        }

        /// <summary>
        /// Gets the identifier that identifies the cause of this changeset.
        /// </summary>
        public Guid CausationId { get; private set; }

        /// <summary>
        /// Gets the identifier that correlates this changeset to other causes.
        /// </summary>
        public Guid CorrelationId { get; private set; }

        /// <summary>
        /// Gets the affected stream changes.
        /// </summary>
        public EventStreamChange[] Changes { get; private set; }
    }
}