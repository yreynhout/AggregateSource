using System;
using System.Collections.Generic;

namespace AggregateSource
{
    /// <summary>
    /// Records events applied to an aggregate's root entity or entity.
    /// </summary>
    public interface IEventRecorder : IEnumerable<object>
    {
        /// <summary>
        /// Records that the specified event happened.
        /// </summary>
        /// <param name="event">The event to record.</param>
        /// <exception cref="ArgumentNullException">Thrown when the specified <paramref name="event"/> is <c>null</c>.</exception>
        void Record(object @event);

        /// <summary>
        /// Resets this instance to its initial state.
        /// </summary>
        void Reset();
    }
}