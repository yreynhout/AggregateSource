namespace AggregateSource.Testing.CollaborationCentric
{
    /// <summary>
    /// Represents a syntactic sugar bootstrapper to compose facts for use in the testing API.
    /// </summary>
    public static class State
    {
        /// <summary>
        /// Defines a set of facts that happened to a particular aggregate.
        /// </summary>
        /// <param name="identifier">The aggregate identifier the events apply to.</param>
        /// <param name="events">The events that occurred.</param>
        /// <returns>A builder of facts.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="identifier"/> or <paramref name="events"/> is <c>null</c>.</exception>
        public static FactsBuilder Fact(string identifier, params object[] events)
        {
            return new FactsBuilder().Fact(identifier, events);
        }

        /// <summary>
        /// Defines a set of facts that happened.
        /// </summary>
        /// <param name="facts">The facts that occurred.</param>
        /// <returns>A builder of facts.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="facts"/> is <c>null</c>.</exception>
        public static FactsBuilder Facts(params Fact[] facts)
        {
            return new FactsBuilder().Facts(facts);
        }
    }
}