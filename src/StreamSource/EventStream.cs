using System;
using System.Collections.Generic;

namespace StreamSource
{
    /// <summary>
    /// Represents the result of reading a stream of events.
    /// </summary>
    public class EventStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStream"/> class.
        /// </summary>
        /// <param name="expectedVersion">The expected version.</param>
        /// <param name="events">The events.</param>
        /// <exception cref="System.ArgumentNullException">events</exception>
        public EventStream(Int32 expectedVersion, IEnumerable<object> events)
        {
            if (events == null) throw new ArgumentNullException("events");
            ExpectedVersion = expectedVersion;
            Events = events;
        }

        /// <summary>
        /// Gets the expected version a stream should be at when we decide to write changes to it.
        /// </summary>
        /// <value>
        /// The expected version.
        /// </value>
        public int ExpectedVersion { get; private set; }

        /// <summary>
        /// Gets the events associated with a stream.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        public IEnumerable<object> Events { get; private set; }
    }
}