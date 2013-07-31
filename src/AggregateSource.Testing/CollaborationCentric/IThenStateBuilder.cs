using System;

namespace AggregateSource.Testing.CollaborationCentric
{
    /// <summary>
    /// The then state within the test specification building process.
    /// </summary>
    public interface IThenStateBuilder : IEventCentricTestSpecificationBuilder
    {
        /// <summary>
        /// Then facts should have occurred.
        /// </summary>
        /// <param name="facts">The facts that should have occurred.</param>
        /// <returns>A builder continuation.</returns>
        IThenStateBuilder Then(params Tuple<string, object>[] facts);

        /// <summary>
        /// Then events should have occurred.
        /// </summary>
        /// <param name="identifier">The aggregate identifier the events belong to.</param>
        /// <param name="events">The events that should have occurred.</param>
        /// <returns>A builder continuation.</returns>
        IThenStateBuilder Then(string identifier, params object[] events);
    }
}