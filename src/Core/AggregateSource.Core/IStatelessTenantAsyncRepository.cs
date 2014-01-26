using System.Threading.Tasks;

namespace AggregateSource
{
    /// <summary>
    /// Represents an asynchronous, virtual collection of <typeparamref name="TAggregateRoot"/> for a particular tenant.
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root in this collection.</typeparam>
    internal interface IStatelessTenantAsyncRepository<TAggregateRoot>
    {
        /// <summary>
        /// Gets the aggregate root entity associated with the specified aggregate identifier.
        /// </summary>
        /// <param name="tenant">The tenant.</param>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <returns>An instance of <typeparamref name="TAggregateRoot"/>.</returns>
        /// <exception cref="AggregateNotFoundException">Thrown when an aggregate is not found.</exception>
        Task<TAggregateRoot> GetAsync(string tenant, string identifier);

        /// <summary>
        /// Gets the aggregate root entity associated with the specified aggregate identifier.
        /// </summary>
        /// <param name="tenant">The tenant.</param>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <param name="version">The aggregate version.</param>
        /// <returns>An instance of <typeparamref name="TAggregateRoot"/>.</returns>
        /// <exception cref="AggregateNotFoundException">Thrown when an aggregate is not found.</exception>
        Task<TAggregateRoot> GetAtAsync(string tenant, string identifier, int version);

        /// <summary>
        /// Attempts to get the aggregate root entity associated with the aggregate identifier.
        /// </summary>
        /// <param name="tenant">The tenant.</param>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <returns>The found <typeparamref name="TAggregateRoot"/>, or empty if not found.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        Task<Optional<TAggregateRoot>> GetOptionalAsync(string tenant, string identifier);

        /// <summary>
        /// Attempts to get the aggregate root entity associated with the aggregate identifier.
        /// </summary>
        /// <param name="tenant">The tenant.</param>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <param name="version">The aggregate version.</param>
        /// <returns>The found <typeparamref name="TAggregateRoot"/>, or empty if not found.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        Task<Optional<TAggregateRoot>> GetOptionalAtAsync(string tenant, string identifier, int version);
    }
}