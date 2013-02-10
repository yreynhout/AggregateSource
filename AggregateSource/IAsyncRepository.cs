using System;
using System.Threading.Tasks;

namespace AggregateSource {
  /// <summary>
  /// Represents a virtual, asynchronously accessible collection of <typeparamref name="TAggregateRoot"/>.
  /// </summary>
  /// <typeparam name="TAggregateRoot">The type of the aggregate root in this collection.</typeparam>
  public interface IAsyncRepository<TAggregateRoot> where TAggregateRoot : AggregateRootEntity {
    /// <summary>
    /// Gets the aggregate root entity associated with the specified aggregate id.
    /// </summary>
    /// <param name="id">The aggregate id.</param>
    /// <returns>A future instance of <typeparamref name="TAggregateRoot"/>.</returns>
    /// <exception cref="AggregateNotFoundException">Thrown when an aggregate is not found.</exception>
    Task<TAggregateRoot> GetAsync(Guid id);
    /// <summary>
    /// Attempts to get the aggregate root entity associated with the aggregate id.
    /// </summary>
    /// <param name="id">The aggregate id.</param>
    /// <returns>The future found <typeparamref name="TAggregateRoot"/>, or a future empty if not found.</returns>
    Task<Optional<TAggregateRoot>> GetOptionalAsync(Guid id);
    /// <summary>
    /// Adds the aggregate root entity to this collection using the specified aggregate id.
    /// </summary>
    /// <param name="id">The aggregate id.</param>
    /// <param name="root">The aggregate root entity.</param>
    void Add(Guid id, TAggregateRoot root);
  }
}
