using System;

namespace AggregateSource {
  public class Repository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : AggregateRootEntity {
    readonly Func<Guid, Aggregate> _reader;
    readonly UnitOfWork _unitOfWork;

    public Repository(Func<Guid, Aggregate> reader, UnitOfWork unitOfWork) {
      if (reader == null) throw new ArgumentNullException("reader");
      if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
      _reader = reader;
      _unitOfWork = unitOfWork;
    }

    public TAggregateRoot Get(Guid id) {
      TAggregateRoot root;
      if(!TryGet(id, out root)) 
        throw new AggregateNotFoundException(id, typeof(TAggregateRoot));
      return root;
    }

    public bool TryGet(Guid id, out TAggregateRoot root) {
      Aggregate aggregate;
      if (_unitOfWork.TryGet(id, out aggregate)) {
        root = (TAggregateRoot)aggregate.Root;
        return true;
      }
      aggregate = _reader(id);
      if (aggregate == null) {
        root = null;
        return false;
      }
      _unitOfWork.Attach(aggregate);
      root = (TAggregateRoot) aggregate.Root;
      return true;
    }

    public void Add(Guid id, TAggregateRoot root) {
      _unitOfWork.Attach(new Aggregate(id, Aggregate.InitialVersion, root));
    }
  }


}