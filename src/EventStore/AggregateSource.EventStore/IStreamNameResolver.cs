namespace AggregateSource.EventStore
{
    /// <summary>
    /// Provides a way to resolve an identifier into a stream's name.
    /// </summary>
    public interface IStreamNameResolver
    {
        /// <summary>
        /// Resolves the specified <paramref name="identifier"/> into a stream name.
        /// </summary>
        /// <param name="identifier">The identifier to resolve.</param>
        /// <returns>The resolved stream name.</returns>
        string Resolve(string identifier);
    }
}