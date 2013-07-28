using System.Collections.Generic;

namespace AggregateSource.GEventStore
{
    /// <summary>
    /// Represents the reading of events associated with an aggregate's underlying stream.
    /// </summary>
    public interface IEventReader {
        /// <summary>
        /// Reads the events associated with the specified aggregate identifier as of the specified version.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <param name="version">The version as of which to read the events.</param>
        /// <returns>An enumeration of <see cref="EventsSlice">event slices</see>.</returns>
        IEnumerable<EventsSlice> Read(string identifier, int version);
    }
}