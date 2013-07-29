using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace AggregateSource.GEventStore
{
    /// <summary>
    /// Represents the reading of events associated with an aggregate's underlying stream.
    /// </summary>
    public class AsyncEventReader : IAsyncEventReader
    {
        readonly IEventStoreConnection _connection;
        readonly EventReaderConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventReader"/> class.
        /// </summary>
        /// <param name="connection">The event store connection.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="connection"/> or <paramref name="configuration"/> is <c>null</c>.</exception>
        public AsyncEventReader(IEventStoreConnection connection, EventReaderConfiguration configuration)
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
        public IAsyncEnumerator<EventsSlice> ReadAsync(string identifier, int version)
        {
            if (identifier == null) throw new ArgumentNullException("identifier");
            return new AsyncEnumerator(identifier, version, _connection, _configuration);
        }

        enum State
        {
            Initial,
            ReadSlice,
            Final
        }

        class AsyncEnumerator : IAsyncEnumerator<EventsSlice>
        {
            readonly string _identifier;
            readonly int _version;
            readonly IEventStoreConnection _connection;
            readonly EventReaderConfiguration _configuration;
            
            State _state;
            UserCredentials _streamUserCredentials;
            string _streamName;
            StreamEventsSlice _slice;
            EventsSlice _current;
            bool _disposed;

            public AsyncEnumerator(string identifier, int version, IEventStoreConnection connection, EventReaderConfiguration configuration)
            {
                _identifier = identifier;
                _version = version;
                _connection = connection;
                _configuration = configuration;
                _state = State.Initial;
                _current = null;
                _disposed = false;
            }

            public EventsSlice Current
            {
                get
                {
                    ThrowIfDisposed();
                    return _current;
                }
            }

            public async Task<bool> MoveNextAsync()
            {
                ThrowIfDisposed();
                var result = false;
                if (_state == State.Initial || _state == State.ReadSlice)
                {
                    switch (_state)
                    {
                        case State.Initial:
                            _streamUserCredentials = _configuration.StreamUserCredentialsResolver.Resolve(_identifier);
                            _streamName = _configuration.StreamNameResolver.Resolve(_identifier);
                            _slice = await _connection.
                                               ReadStreamEventsForwardAsync(
                                                   _streamName,
                                                   _version,
                                                   _configuration.SliceSize,
                                                   false,
                                                   _streamUserCredentials);
                            break;
                        case State.ReadSlice:
                            _slice = await _connection.
                                               ReadStreamEventsForwardAsync(
                                                   _streamName,
                                                   _slice.NextEventNumber,
                                                   _configuration.SliceSize,
                                                   false,
                                                   _streamUserCredentials);
                            break;
                    }
                    switch (_slice.Status)
                    {
                        case SliceReadStatus.StreamDeleted:
                            _current = EventsSlice.Deleted;
                            break;
                        case SliceReadStatus.StreamNotFound:
                            _current = EventsSlice.NotFound;
                            break;
                        case SliceReadStatus.Success:
                            _current = new EventsSlice(
                                _slice.Status,
                                _slice.Events.Select(resolved => _configuration.Deserializer.Deserialize(resolved)).ToArray(),
                                _slice.LastEventNumber);
                            break;
                        default:
                            throw new NotSupportedException(
                                string.Format("The specified slice read status {0} is currently not supported.", _slice.Status));
                    }
                    _state = _slice.IsEndOfStream ? State.Final : State.ReadSlice;
                    result = true;
                }
                return result;
            }

            void ThrowIfDisposed()
            {
                if(_disposed)
                    throw new ObjectDisposedException(typeof(AsyncEnumerator).Name);
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;
                }
            }
        }
    }
}