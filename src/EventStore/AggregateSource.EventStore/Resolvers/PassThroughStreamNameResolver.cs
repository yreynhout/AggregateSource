using System;

namespace AggregateSource.EventStore.Resolvers
{
    /// <summary>
    /// Stream name resolver that can be used when the identifier is the stream name.
    /// </summary>
    public class PassThroughStreamNameResolver : IStreamNameResolver
    {
        /// <summary>
        /// Resolves the specified <paramref name="identifier" /> into a stream name.
        /// </summary>
        /// <param name="identifier">The identifier to resolve.</param>
        /// <returns>
        /// The resolved stream name.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="identifier"/> is <c>null</c>.</exception>
        public string Resolve(string identifier)
        {
            if (identifier == null) throw new ArgumentNullException("identifier");
            return identifier;
        }
    }
}