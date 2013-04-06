using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore {
  public class AsyncRepository<TAggregateRoot> : IAsyncRepository<TAggregateRoot> where TAggregateRoot : IAggregateRootEntity {
    readonly Func<TAggregateRoot> _rootFactory;
    readonly ConcurrentUnitOfWork _unitOfWork;
    readonly EventStoreConnection _connection;
    readonly EventStoreReadConfiguration _configuration;

    public AsyncRepository(Func<TAggregateRoot> rootFactory, ConcurrentUnitOfWork unitOfWork, EventStoreConnection connection)
      : this(rootFactory, unitOfWork, connection, EventStoreReadConfiguration.Default) {
    }

    public AsyncRepository(Func<TAggregateRoot> rootFactory, ConcurrentUnitOfWork unitOfWork, EventStoreConnection connection, EventStoreReadConfiguration configuration) {
      if (rootFactory == null) throw new ArgumentNullException("rootFactory");
      if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
      if (connection == null) throw new ArgumentNullException("connection");
      if (configuration == null) throw new ArgumentNullException("configuration");
      _rootFactory = rootFactory;
      _unitOfWork = unitOfWork;
      _connection = connection;
      _configuration = configuration;
    }

    public async Task<TAggregateRoot> GetAsync(string identifier) {
      var result = await GetOptionalAsync(identifier);
      if (!result.HasValue)
        throw new AggregateNotFoundException(identifier, typeof(TAggregateRoot));
      return result.Value;
    }

    public async Task<Optional<TAggregateRoot>> GetOptionalAsync(string identifier) {
      Aggregate aggregate;
      if (_unitOfWork.TryGet(identifier, out aggregate)) {
        return new Optional<TAggregateRoot>((TAggregateRoot)aggregate.Root);
      }
      var slice = await _connection.ReadStreamEventsForwardAsync(identifier, 1, _configuration.SliceSize, false);
      if (slice.Status == SliceReadStatus.StreamDeleted || slice.Status == SliceReadStatus.StreamNotFound) {
        return Optional<TAggregateRoot>.Empty;
      }
      var root = _rootFactory();
      root.Initialize(slice.Events.Select(resolved => _configuration.Deserializer.Deserialize(resolved)));
      while (!slice.IsEndOfStream) {
        slice = await _connection.ReadStreamEventsForwardAsync(identifier, slice.NextEventNumber, _configuration.SliceSize, false);
        root.Initialize(slice.Events.Select(resolved => _configuration.Deserializer.Deserialize(resolved)));
      }
      aggregate = new Aggregate(identifier, slice.LastEventNumber, root);
      _unitOfWork.Attach(aggregate);
      return new Optional<TAggregateRoot>(root);
    }

    public void Add(string identifier, TAggregateRoot root) {
      _unitOfWork.Attach(new Aggregate(identifier, ExpectedVersion.NoStream, root));
    }
  }
}