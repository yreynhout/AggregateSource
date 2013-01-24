using System;

namespace AggregateSource {
  public abstract class Repository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : AggregateRootEntity {
    readonly UnitOfWork _unitOfWork;

    protected Repository(UnitOfWork unitOfWork) {
      if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
      _unitOfWork = unitOfWork;
    }

    protected UnitOfWork UnitOfWork {
      get { return _unitOfWork; }
    }

    public TAggregateRoot Get(Guid id) {
      TAggregateRoot root;
      if (!TryGet(id, out root))
        throw new AggregateNotFoundException(id, typeof(TAggregateRoot));
      return root;
    }

    public bool TryGet(Guid id, out TAggregateRoot root) {
      Aggregate aggregate;
      if (UnitOfWork.TryGet(id, out aggregate)) {
        root = (TAggregateRoot)aggregate.Root;
        return true;
      }
      if (!TryReadAggregate(id, out aggregate)) {
        root = null;
        return false;
      }
      UnitOfWork.Attach(aggregate);
      root = (TAggregateRoot)aggregate.Root;
      return true;
    }

    public void Add(Guid id, TAggregateRoot root) {
      UnitOfWork.Attach(CreateAggregate(id, root));
    }

    protected abstract bool TryReadAggregate(Guid id, out Aggregate aggregate);
    protected abstract Aggregate CreateAggregate(Guid id, TAggregateRoot root);
  }

  //public class VersionedAggregateReader<TAggregateRoot> : 
  //  IAggregateReader<TAggregateRoot>, 
  //  IAggregateFactory<TAggregateRoot> 
  //  where TAggregateRoot : AggregateRootEntity{
  //  readonly Func<TAggregateRoot> _rootFactory;

  //  public VersionedAggregateReader (Func<TAggregateRoot> aggregateRootFactory) {
  //    if (aggregateRootFactory == null) throw new ArgumentNullException("aggregateRootFactory");
  //    _rootFactory = aggregateRootFactory;
  //  }

  //  public Aggregate Read(Guid id) {
  //    var root = _rootFactory();
  //    root.Initialize(...);
  //    return new VersionedAggregate(id, version, root);
  //  }

  //  public Aggregate Create(Guid id, TAggregateRoot root) {
  //    return new VersionedAggregate(id);
  //  }
  //}

  //public class VersionedAggregate : Aggregate {
  //  public static readonly int InitialVersion = 0;

  //  public VersionedAggregate(Guid id, int version, AggregateRootEntity root) : base(id, version, root) {

  //  }
  //}
}