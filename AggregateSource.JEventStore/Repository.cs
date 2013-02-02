using System;
using System.Linq;
using EventStore;

namespace AggregateSource.JEventStore {
  public class Repository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : AggregateRootEntity {
    readonly Func<TAggregateRoot> _rootFactory;
    readonly UnitOfWork _unitOfWork;
    readonly ICommitEvents _commitReader;

    public Repository(Func<TAggregateRoot> rootFactory, UnitOfWork unitOfWork, ICommitEvents commitReader) {
      if (rootFactory == null) throw new ArgumentNullException("rootFactory");
      if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
      if (commitReader == null) throw new ArgumentNullException("commitReader");
      _rootFactory = rootFactory;
      _unitOfWork = unitOfWork;
      _commitReader = commitReader;
    }

    public TAggregateRoot Get(Guid id) {
      TAggregateRoot root;
      if (!TryGet(id, out root))
        throw new AggregateNotFoundException(id, typeof(TAggregateRoot));
      return root;
    }

    public bool TryGet(Guid id, out TAggregateRoot root) {
      Aggregate aggregate;
      if (_unitOfWork.TryGet(id, out aggregate)) {
        root = (TAggregateRoot) aggregate.Root;
        return true;
      }
      var version = 0;
      using (var enumerator = _commitReader.GetFrom(id, 0, Int32.MaxValue).GetEnumerator()) {
        var moved = enumerator.MoveNext();
        if (!moved) {
          root = null;
          return false;
        }
        root = _rootFactory();
        while (moved) {
          root.Initialize(enumerator.Current.Events.Select(eventMessage => eventMessage.Body));
          version = enumerator.Current.StreamRevision;
          moved = enumerator.MoveNext();
        }
      }
      aggregate = new Aggregate(id, version, root);
      _unitOfWork.Attach(aggregate);
      return true;
    }

    public void Add(Guid id, TAggregateRoot root) {
      _unitOfWork.Attach(new Aggregate(id, Aggregate.InitialVersion, root));
    }
  }
}
