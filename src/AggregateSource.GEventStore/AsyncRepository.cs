using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore {
  /// <summary>
  /// Represents an asynchronous, virtual collection of <typeparamref name="TAggregateRoot"/>.
  /// </summary>
  /// <typeparam name="TAggregateRoot">The type of the aggregate root in this collection.</typeparam>
  public class AsyncRepository<TAggregateRoot> : IAsyncRepository<TAggregateRoot> where TAggregateRoot : IAggregateRootEntity {
    readonly Func<TAggregateRoot> _rootFactory;
    readonly ConcurrentUnitOfWork _unitOfWork;
    readonly EventStoreConnection _connection;
    readonly EventReaderConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRepository{TAggregateRoot}"/> class.
    /// </summary>
    /// <param name="rootFactory">The aggregate root entity factory.</param>
    /// <param name="unitOfWork">The unit of work to interact with.</param>
    /// <param name="connection">The event store connection to use.</param>
    /// <param name="configuration">The event store configuration to use.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="rootFactory"/> or <paramref name="unitOfWork"/> or <paramref name="connection"/> or <paramref name="configuration"/> is null.</exception>
    public AsyncRepository(Func<TAggregateRoot> rootFactory, ConcurrentUnitOfWork unitOfWork, EventStoreConnection connection, EventReaderConfiguration configuration) {
      if (rootFactory == null) throw new ArgumentNullException("rootFactory");
      if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
      if (connection == null) throw new ArgumentNullException("connection");
      if (configuration == null) throw new ArgumentNullException("configuration");
      _rootFactory = rootFactory;
      _unitOfWork = unitOfWork;
      _connection = connection;
      _configuration = configuration;
    }

    /// <summary>
    /// Gets the aggregate root entity associated with the specified aggregate identifier.
    /// </summary>
    /// <param name="identifier">The aggregate identifier.</param>
    /// <returns>An instance of <typeparamref name="TAggregateRoot"/>.</returns>
    /// <exception cref="AggregateNotFoundException">Thrown when an aggregate is not found.</exception>
    public async Task<TAggregateRoot> GetAsync(string identifier) {
      var result = await GetOptionalAsync(identifier);
      if (!result.HasValue)
        throw new AggregateNotFoundException(identifier, typeof(TAggregateRoot));
      return result.Value;
    }

    /// <summary>
    /// Attempts to get the aggregate root entity associated with the aggregate identifier.
    /// </summary>
    /// <param name="identifier">The aggregate identifier.</param>
    /// <returns>The found <typeparamref name="TAggregateRoot"/>, or empty if not found.</returns>
    public async Task<Optional<TAggregateRoot>> GetOptionalAsync(string identifier) {
      Aggregate aggregate;
      if (_unitOfWork.TryGet(identifier, out aggregate)) {
        return new Optional<TAggregateRoot>((TAggregateRoot)aggregate.Root);
      }
      var streamName = _configuration.Resolver.Resolve(identifier);
      var slice = await _connection.ReadStreamEventsForwardAsync(streamName, 1, _configuration.SliceSize, false);
      if (slice.Status == SliceReadStatus.StreamDeleted || slice.Status == SliceReadStatus.StreamNotFound) {
        return Optional<TAggregateRoot>.Empty;
      }
      var root = _rootFactory();
      root.Initialize(slice.Events.Select(resolved => _configuration.Deserializer.Deserialize(resolved)));
      while (!slice.IsEndOfStream) {
        slice = await _connection.ReadStreamEventsForwardAsync(streamName, slice.NextEventNumber, _configuration.SliceSize, false);
        root.Initialize(slice.Events.Select(resolved => _configuration.Deserializer.Deserialize(resolved)));
      }
      aggregate = new Aggregate(identifier, slice.LastEventNumber, root);
      _unitOfWork.Attach(aggregate);
      return new Optional<TAggregateRoot>(root);
    }

    /// <summary>
    /// Adds the aggregate root entity to this collection using the specified aggregate identifier.
    /// </summary>
    /// <param name="identifier">The aggregate identifier.</param>
    /// <param name="root">The aggregate root entity.</param>
    public void Add(string identifier, TAggregateRoot root) {
      _unitOfWork.Attach(new Aggregate(identifier, ExpectedVersion.NoStream, root));
    }
  }
}