using System.Collections.Generic;

namespace AggregateSource
{
    /// <summary>
    /// Initializes an aggregate.
    /// </summary>
    public interface IAggregateInitializer
    {
        /// <summary>
        /// Initializes this instance using the specified events.
        /// </summary>
        /// <param name="events">The events to initialize with.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="events"/> are null.</exception>
        void Initialize(IEnumerable<object> events);
    }
}