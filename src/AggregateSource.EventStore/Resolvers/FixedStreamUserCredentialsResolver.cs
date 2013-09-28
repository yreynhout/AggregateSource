using System;
using EventStore.ClientAPI.SystemData;

namespace AggregateSource.EventStore.Resolvers
{
    /// <summary>
    /// Stream user credentials resolver that can be used when all streams are accessed using the same user credentials.
    /// </summary>
    public class FixedStreamUserCredentialsResolver : IStreamUserCredentialsResolver
    {
        readonly UserCredentials _fixedUserCredentials;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedStreamUserCredentialsResolver"/> class.
        /// </summary>
        /// <param name="fixedUserCredentials">The fixed user credentials.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="fixedUserCredentials"/> is <c>null</c>.</exception>
        public FixedStreamUserCredentialsResolver(UserCredentials fixedUserCredentials)
        {
            if (fixedUserCredentials == null) throw new ArgumentNullException("fixedUserCredentials");
            _fixedUserCredentials = fixedUserCredentials;
        }

        /// <summary>
        /// Resolves the specified <paramref name="identifier" /> into user credentials used to access the associated stream.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns>
        /// The associated <see cref="UserCredentials" />.
        /// </returns>
        public UserCredentials Resolve(string identifier)
        {
            if (identifier == null) throw new ArgumentNullException("identifier");
            return _fixedUserCredentials;
        }
    }
}