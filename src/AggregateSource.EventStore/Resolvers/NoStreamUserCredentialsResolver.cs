using System;
using EventStore.ClientAPI.SystemData;

namespace AggregateSource.EventStore.Resolvers
{
    /// <summary>
    /// Stream user credentials resolver that can be used when all streams are accessed without credentials.
    /// </summary>
    public class NoStreamUserCredentialsResolver : IStreamUserCredentialsResolver
    {
        /// <summary>
        /// Resolves the specified <paramref name="identifier" /> into user credentials used to access the associated stream.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns>
        /// The associated <see cref="UserCredentials" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">identifier</exception>
        public UserCredentials Resolve(string identifier)
        {
            if (identifier == null) throw new ArgumentNullException("identifier");
            return null;
        }
    }
}