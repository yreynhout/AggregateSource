using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AggregateSource {
  /// <summary>
  /// Tracks changes of attached aggregates.
  /// </summary>
  public class ConcurrentUnitOfWork {
    readonly ConcurrentDictionary<string, Aggregate> _aggregates;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrentUnitOfWork"/> class.
    /// </summary>
    public ConcurrentUnitOfWork() {
      _aggregates = new ConcurrentDictionary<string, Aggregate>();
    }

    /// <summary>
    /// Attaches the specified aggregate.
    /// </summary>
    /// <param name="aggregate">The aggregate.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="aggregate"/> is null.</exception>
    public void Attach(Aggregate aggregate) {
      if (aggregate == null)
        throw new ArgumentNullException("aggregate");
      if (!_aggregates.TryAdd(aggregate.Identifier, aggregate))
        throw new ArgumentException(string.Format("The aggregate of type '{0}' with identifier '{1}' was already added. This indicates could indicate there's a race condition, i.e. the same aggregate gets attached multiple times.", aggregate.Root.GetType().Name, aggregate.Identifier));
    }

    /// <summary>
    /// Attempts to get the <see cref="Aggregate"/> using the specified aggregate identifier.
    /// </summary>
    /// <param name="identifier">The aggregate identifier.</param>
    /// <param name="aggregate">The aggregate if found, otherwise <c>null</c>.</param>
    /// <returns><c>true</c> if the aggregate was found, otherwise <c>false</c>.</returns>
    public bool TryGet(string identifier, out Aggregate aggregate) {
      return _aggregates.TryGetValue(identifier, out aggregate);
    }

    /// <summary>
    /// Determines whether this instance has aggregates with state changes.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance has aggregates with state changes; otherwise, <c>false</c>.
    /// </returns>
    public bool HasChanges() {
      return _aggregates.Values.Any(aggregate => aggregate.Root.HasChanges());
    }

    /// <summary>
    /// Gets the aggregates with state changes.
    /// </summary>
    /// <returns>An enumeration of <see cref="Aggregate"/>.</returns>
    public IEnumerable<Aggregate> GetChanges() {
      return _aggregates.Values.Where(aggregate => aggregate.Root.HasChanges());
    }
  }
}