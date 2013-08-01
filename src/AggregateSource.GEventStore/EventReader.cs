using System;
using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore
{
    /// <summary>
    /// Represents the reading of events associated with an aggregate's underlying stream.
    /// </summary>
    public class EventReader : IEventReader
    {
        readonly IEventStoreConnection _connection;
        readonly EventReaderConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventReader"/> class.
        /// </summary>
        /// <param name="connection">The event store connection.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="connection"/> or <paramref name="configuration"/> is <c>null</c>.</exception>
        public EventReader(IEventStoreConnection connection, EventReaderConfiguration configuration)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (configuration == null) throw new ArgumentNullException("configuration");
            _connection = connection;
            _configuration = configuration;
        }

        /// <summary>
        /// Reads the events associated with the specified aggregate identifier as of the specified version.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <param name="version">The version as of which to read the events.</param>
        /// <returns>
        /// An enumeration of <see cref="EventsSlice">event slices</see>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="identifier"/> is <c>null</c>.</exception>
        /// <exception cref="System.NotSupportedException">Thrown when an unsupported <see cref="SliceReadStatus"/> is encountered.</exception>
        public IEnumerable<EventsSlice> Read(string identifier, int version)
        {
            if (identifier == null) throw new ArgumentNullException("identifier");
            if (version <= 0) throw new ArgumentOutOfRangeException("version");
            return ReadCore(identifier, version);
        }

        IEnumerable<EventsSlice> ReadCore(string identifier, int version)
        {
            var streamUserCredentials = _configuration.StreamUserCredentialsResolver.Resolve(identifier);
            var streamName = _configuration.StreamNameResolver.Resolve(identifier);
            var slice = _connection.
                ReadStreamEventsForward(
                    streamName,
                    version,
                    _configuration.SliceSize,
                    false,
                    streamUserCredentials);
            do
            {
                switch (slice.Status)
                {
                    case SliceReadStatus.StreamDeleted:
                        yield return EventsSlice.Deleted;
                        break;
                    case SliceReadStatus.StreamNotFound:
                        yield return EventsSlice.NotFound;
                        break;
                    case SliceReadStatus.Success:
                        yield return new EventsSlice(
                            slice.Status,
                            slice.Events.Select(resolved => _configuration.Deserializer.Deserialize(resolved)).ToArray(),
                            slice.LastEventNumber);
                        break;
                    default:
                        throw new NotSupportedException(
                            string.Format("The specified slice read status {0} is currently not supported.", slice.Status));
                }
                slice = _connection.
                    ReadStreamEventsForward(
                        streamName,
                        slice.NextEventNumber,
                        _configuration.SliceSize,
                        false,
                        streamUserCredentials);
            } while (!slice.IsEndOfStream && slice.Status == SliceReadStatus.Success);
        }
    }
}