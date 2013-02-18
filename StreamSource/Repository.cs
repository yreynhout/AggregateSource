using System;
using AggregateSource;

namespace StreamSource {
  /// <summary>
  /// Represents a default repository implementation.
  /// </summary>
  /// <typeparam name="TAggregateRoot">Type of the aggregate root entity.</typeparam>
  public class Repository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : IAggregateRootEntity {
    readonly Func<TAggregateRoot> _rootFactory;
    readonly UnitOfWork _unitOfWork;
    readonly IEventStreamReader _eventStreamReader;

    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{TAggregateRoot}"/> class.
    /// </summary>
    /// <param name="rootFactory">The aggregate root entity factory.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="eventStreamReader">The event stream reader.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="rootFactory"/> or the <paramref name="unitOfWork"/> or the <paramref name="eventStreamReader"/> is null.</exception>
    public Repository(Func<TAggregateRoot> rootFactory, UnitOfWork unitOfWork, IEventStreamReader eventStreamReader) {
      if (rootFactory == null) throw new ArgumentNullException("rootFactory");
      if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
      if (eventStreamReader == null) throw new ArgumentNullException("eventStreamReader");
      _rootFactory = rootFactory;
      _unitOfWork = unitOfWork;
      _eventStreamReader = eventStreamReader;
    }

    /// <summary>
    /// Gets the aggregate root entity associated with the specified aggregate id.
    /// </summary>
    /// <param name="id">The aggregate id.</param>
    /// <returns>An instance of <typeparamref name="TAggregateRoot"/>.</returns>
    /// <exception cref="AggregateNotFoundException">Thrown when an aggregate is not found.</exception>
    public TAggregateRoot Get(Guid id) {
      var result = GetOptional(id);
      if (!result.HasValue)
        throw new AggregateNotFoundException(id, typeof(TAggregateRoot));
      return result.Value;
    }

    /// <summary>
    /// Attempts to get the aggregate root entity associated with the aggregate id.
    /// </summary>
    /// <param name="id">The aggregate id.</param>
    /// <returns>The found <typeparamref name="TAggregateRoot"/>, or empty if not found.</returns>
    public Optional<TAggregateRoot> GetOptional(Guid id) {
      Aggregate aggregate;
      if (_unitOfWork.TryGet(id, out aggregate)) {
        return new Optional<TAggregateRoot>((TAggregateRoot)aggregate.Root);
      }
      var result = _eventStreamReader.Read(id);
      if (!result.HasValue) {
        return Optional<TAggregateRoot>.Empty;
      }
      var eventStream = result.Value;
      var root = _rootFactory();
      root.Initialize(eventStream.Events);
      aggregate = new Aggregate(id, eventStream.ExpectedVersion, root);
      _unitOfWork.Attach(aggregate);
      return new Optional<TAggregateRoot>(root);
    }

    /// <summary>
    /// Adds the aggregate root entity to this collection using the specified aggregate id.
    /// </summary>
    /// <param name="id">The aggregate id.</param>
    /// <param name="root">The aggregate root entity.</param>
    public void Add(Guid id, TAggregateRoot root) {
      _unitOfWork.Attach(new Aggregate(id, -1, root));
    }
  }
}