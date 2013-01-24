using System;

namespace AggregateSource {
  /// <summary>
  /// Represents a way to retrieve from and add to the collection of <typeparamref name="TAggregateRoot"/>.
  /// </summary>
  /// <typeparam name="TAggregateRoot">The type of the aggregate root in this collection.</typeparam>
  public interface IRepository<TAggregateRoot> where TAggregateRoot : AggregateRootEntity {
    /// <summary>
    /// Gets the aggregate root entity associated with the specified aggregate id.
    /// </summary>
    /// <param name="id">The aggregate id.</param>
    /// <returns>An instance of <typeparamref name="TAggregateRoot"/>.</returns>
    /// <exception cref="AggregateNotFoundException">Thrown when an aggregate is not found.</exception>
    TAggregateRoot Get(Guid id);

    /// <summary>
    /// Attempts to get the aggregate root entity associated with the aggregate id.
    /// </summary>
    /// <param name="id">The aggregate id.</param>
    /// <param name="root">The found <typeparamref name="TAggregateRoot"/>, or <c>null</c> if not found.</param>
    /// <returns><c>true</c> if the aggregate is found, otherwise <c>false</c>.</returns>
    bool TryGet(Guid id, out TAggregateRoot root);

    /// <summary>
    /// Adds the aggregate root entity to this collection using the specified aggregate id.
    /// </summary>
    /// <param name="id">The aggregate id.</param>
    /// <param name="root">The aggregate root entity.</param>
    void Add(Guid id, TAggregateRoot root);
  }
}