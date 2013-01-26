using System;
using AggregateSource.Ambient.Properties;

namespace AggregateSource.Ambient {
  /// <summary>
  /// Base class for repositories.
  /// </summary>
  /// <typeparam name="TAggregateRoot">Type of the aggregate root entity.</typeparam>
  public abstract class AmbientRepository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : AggregateRootEntity {
    readonly IAmbientUnitOfWorkStore _store;

    /// <summary>
    /// Initializes a new instance of the <see cref="AmbientRepository{TAggregateRoot}"/> class.
    /// </summary>
    /// <param name="store">The local store of the unit of work.</param>
    /// <remarks>Use this constructor if you want to use an ambient unit of work.</remarks>
    protected AmbientRepository(IAmbientUnitOfWorkStore store) {
      if (store == null) throw new ArgumentNullException("store");
      _store = store;
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
      if (AmbientUnitOfWork.TryGet(id, out aggregate)) {
        root = (TAggregateRoot)aggregate.Root;
        return true;
      }
      if (!TryReadAggregate(id, out aggregate)) {
        root = null;
        return false;
      }
      AmbientUnitOfWork.Attach(aggregate);
      root = (TAggregateRoot)aggregate.Root;
      return true;
    }

    /// <summary>
    /// Adds the aggregate root entity to this collection using the specified aggregate id.
    /// </summary>
    /// <param name="id">The aggregate id.</param>
    /// <param name="root">The aggregate root entity.</param>
    public void Add(Guid id, TAggregateRoot root) {
      AmbientUnitOfWork.Attach(CreateAggregate(id, root));
    }

    UnitOfWork AmbientUnitOfWork {
      get {
        UnitOfWork unitOfWork;
        if(!_store.TryGet(out unitOfWork))
          throw new UnitOfWorkScopeException(Resources.Repository_NoAmbientUnitOfWork);
        return unitOfWork;
      }
    }

    /// <summary>
    /// Attempts to read the aggregate from underlying storage.
    /// </summary>
    /// <param name="id">The aggregate id.</param>
    /// <param name="aggregate">The found aggregate, or <c>null</c>.</param>
    /// <returns><c>true</c> if the aggregate was found, otherwise <c>false</c>.</returns>
    protected abstract bool TryReadAggregate(Guid id, out Aggregate aggregate);


    /// <summary>
    /// Creates an aggregate instance to be attached to the <see cref="UnitOfWork"/>.
    /// </summary>
    /// <param name="id">The aggregate id.</param>
    /// <param name="root">The aggregate root entity.</param>
    /// <returns>A new <see cref="Aggregate"/>.</returns>
    protected abstract Aggregate CreateAggregate(Guid id, TAggregateRoot root);
  }
}