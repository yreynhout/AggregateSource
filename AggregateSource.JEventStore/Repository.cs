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
      var result = GetOptional(id);
      if (!result.HasValue)
        throw new AggregateNotFoundException(id, typeof(TAggregateRoot));
      return result.Value;
    }

    public Optional<TAggregateRoot> GetOptional(Guid id) {
      Aggregate aggregate;
      if (_unitOfWork.TryGet(id, out aggregate)) {
        return new Optional<TAggregateRoot>((TAggregateRoot)aggregate.Root);
      }
      TAggregateRoot root;
      var version = 0;
      using (var enumerator = _commitReader.GetFrom(id, 0, Int32.MaxValue).GetEnumerator()) {
        var moved = enumerator.MoveNext();
        if (!moved) {
          return Optional<TAggregateRoot>.Empty;
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
      return new Optional<TAggregateRoot>(root);
    }

    public void Add(Guid id, TAggregateRoot root) {
      _unitOfWork.Attach(new Aggregate(id, 0, root));
    }
  }
}
