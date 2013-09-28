using EventStore.ClientAPI.SystemData;

namespace AggregateSource.EventStore
{
    /// <summary>
    /// Provides a way to resolve an identifier into the user credentials used to access the associated stream.
    /// </summary>
    public interface IStreamUserCredentialsResolver
    {
        /// <summary>
        /// Resolves the specified <paramref name="identifier"/> into user credentials used to access the associated stream.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns>The associated <see cref="UserCredentials"/>.</returns>
        UserCredentials Resolve(string identifier);
    }
}