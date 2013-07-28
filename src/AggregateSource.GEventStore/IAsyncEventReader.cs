namespace AggregateSource.GEventStore
{
    /// <summary>
    /// Represents the asynchronous reading of events associated with an aggregate's underlying stream.
    /// </summary>
    public interface IAsyncEventReader
    {
        /// <summary>
        /// Reads the events associated with the specified aggregate identifier as of the specified version.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <param name="version">The version as of which to read the events.</param>
        /// <returns>An asynchronous enumeration of <see cref="EventsSlice">event slices</see>.</returns>
        IAsyncEnumerator<EventsSlice> ReadAsync(string identifier, int version);
    }
}