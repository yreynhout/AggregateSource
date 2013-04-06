using System;
using System.Collections.Generic;
using System.Linq;

namespace AggregateSource {
  /// <summary>
  /// Tracks changes of attached aggregates.
  /// </summary>
  public class UnitOfWork {
    readonly Dictionary<string, Aggregate> _aggregates;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    public UnitOfWork() {
      _aggregates = new Dictionary<string, Aggregate>();
    }

    /// <summary>
    /// Attaches the specified aggregate.
    /// </summary>
    /// <param name="aggregate">The aggregate.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="aggregate"/> is null.</exception>
    public void Attach(Aggregate aggregate) {
      if (aggregate == null) 
        throw new ArgumentNullException("aggregate");
      _aggregates.Add(aggregate.Identifier, aggregate);
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