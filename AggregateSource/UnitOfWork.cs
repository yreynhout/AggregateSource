using System;
using System.Collections.Generic;
using System.Linq;

namespace AggregateSource {
  public class UnitOfWork {
    readonly Dictionary<Guid, Aggregate> _aggregates;

    public UnitOfWork() {
      _aggregates = new Dictionary<Guid, Aggregate>();
    }

    public void Attach(Aggregate aggregate) {
      if (aggregate == null) throw new ArgumentNullException("aggregate");
      _aggregates.Add(aggregate.Id, aggregate);
    }

    public bool TryGet(Guid id, out Aggregate aggregate) {
      return _aggregates.TryGetValue(id, out aggregate);
    }

    public bool HasChanges() {
      return _aggregates.Values.Any(aggregate => aggregate.Root.HasChanges());
    }

    public IEnumerable<Aggregate> GetChanges() {
      return _aggregates.Values.Where(aggregate => aggregate.Root.HasChanges());
    }
  }
}