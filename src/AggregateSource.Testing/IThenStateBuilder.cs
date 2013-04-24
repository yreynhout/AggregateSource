using System;

namespace AggregateSource.Testing {
  /// <summary>
  /// The then state within the test specification building process.
  /// </summary>
  public interface IThenStateBuilder : IEventCentricTestSpecificationBuilder {
    /// <summary>
    /// Then events should have occurred.
    /// </summary>
    /// <param name="identifier">The aggregate identifier the events belong to.</param>
    /// <param name="events">The events that should have occurred.</param>
    /// <returns>A builder continuation.</returns>
    IThenStateBuilder Then(string identifier, params object[] events);
  }
}