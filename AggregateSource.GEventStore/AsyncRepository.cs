using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore {
  public class AsyncRepository<TAggregateRoot> : IAsyncRepository<TAggregateRoot> where TAggregateRoot : AggregateRootEntity {
    readonly Func<TAggregateRoot> _rootFactory;
    readonly UnitOfWork _unitOfWork;
    readonly EventStoreConnection _connection;
    readonly EventStoreReadConfiguration _configuration;

    public AsyncRepository(Func<TAggregateRoot> rootFactory, UnitOfWork unitOfWork, EventStoreConnection connection)
      : this(rootFactory, unitOfWork, connection, EventStoreReadConfiguration.Default) {
    }

    public AsyncRepository(Func<TAggregateRoot> rootFactory, UnitOfWork unitOfWork, EventStoreConnection connection, EventStoreReadConfiguration configuration) {
      if (rootFactory == null) throw new ArgumentNullException("rootFactory");
      if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
      if (connection == null) throw new ArgumentNullException("connection");
      if (configuration == null) throw new ArgumentNullException("configuration");
      _rootFactory = rootFactory;
      _unitOfWork = unitOfWork;
      _connection = connection;
      _configuration = configuration;
    }

    public async Task<TAggregateRoot> GetAsync(Guid id) {
      var result = await GetOptionalAsync(id);
      if(!result.HasValue)
        throw new AggregateNotFoundException(id, typeof(TAggregateRoot));
      return result.Value;
    }

    public async Task<Optional<TAggregateRoot>> GetOptionalAsync(Guid id) {
      Aggregate aggregate;
      TAggregateRoot root;
      if (_unitOfWork.TryGet(id, out aggregate)) {
        root = (TAggregateRoot)aggregate.Root;
        return new Optional<TAggregateRoot>(root);
      }
      var stream = StreamName.Create<TAggregateRoot>(id);
      var slice = await _connection.ReadStreamEventsForwardAsync(stream, 1, _configuration.SliceSize, false);
      if (slice.Status == SliceReadStatus.StreamDeleted || slice.Status == SliceReadStatus.StreamNotFound) {
        return Optional<TAggregateRoot>.Empty;
      }
      root = _rootFactory();
      root.Initialize(slice.Events.Select(resolved => _configuration.Deserializer.Deserialize(resolved)));
      while (!slice.IsEndOfStream) {
        slice = await _connection.ReadStreamEventsForwardAsync(stream, slice.NextEventNumber, _configuration.SliceSize, false);
        root.Initialize(slice.Events.Select(resolved => _configuration.Deserializer.Deserialize(resolved)));
      }
      aggregate = new Aggregate(id, slice.LastEventNumber, root);
      _unitOfWork.Attach(aggregate); //Lovely multi-threading bug
      return new Optional<TAggregateRoot>(root);
    }

    public void Add(Guid id, TAggregateRoot root) {
      _unitOfWork.Attach(new Aggregate(id, ExpectedVersion.NoStream, root));
    }
  }
}