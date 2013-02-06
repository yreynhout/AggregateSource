using System;
using System.Collections.Generic;

namespace AggregateSource {
  /// <summary>
  /// Represents the default repository implementation.
  /// </summary>
  /// <typeparam name="TAggregateRoot">Type of the aggregate root entity.</typeparam>
  public class Repository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : AggregateRootEntity {
    readonly Func<TAggregateRoot> _rootFactory;
    readonly UnitOfWork _unitOfWork;
    readonly Func<Guid, Tuple<Int32, IEnumerable<object>>> _eventStreamReader;

    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{TAggregateRoot}"/> class.
    /// </summary>
    /// <param name="rootFactory">The aggregate root entity factory.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="eventStreamReader">The event stream reader.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="rootFactory"/> or the <paramref name="unitOfWork"/> or the <paramref name="eventStreamReader"/> is null.</exception>
    public Repository(Func<TAggregateRoot> rootFactory, UnitOfWork unitOfWork, Func<Guid, Tuple<Int32, IEnumerable<object>>> eventStreamReader) {
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
      TAggregateRoot root;
      if (!TryGet(id, out root))
        throw new AggregateNotFoundException(id, typeof(TAggregateRoot));
      return root;
    }

    /// <summary>
    /// Attempts to get the aggregate root entity associated with the aggregate id.
    /// </summary>
    /// <param name="id">The aggregate id.</param>
    /// <param name="root">The found <typeparamref name="TAggregateRoot"/>, or <c>null</c> if not found.</param>
    /// <returns><c>true</c> if the aggregate is found, otherwise <c>false</c>.</returns>
    public bool TryGet(Guid id, out TAggregateRoot root) {
      Aggregate aggregate;
      if (_unitOfWork.TryGet(id, out aggregate)) {
        root = (TAggregateRoot)aggregate.Root;
        return true;
      }
      var eventStream = _eventStreamReader(id);
      if (eventStream == null) {
        root = null;
        return false;
      }
      root = _rootFactory();
      root.Initialize(eventStream.Item2);
      aggregate = new Aggregate(id, eventStream.Item1, root);
      _unitOfWork.Attach(aggregate);
      return true;
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