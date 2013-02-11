using System;

namespace AggregateSource.Testing {
  /// <summary>
  /// The then state within the test specification building process.
  /// </summary>
  public interface IThenStateBuilder {
    /// <summary>
    /// Then events should have occurred.
    /// </summary>
    /// <param name="id">The aggregate those events should have occurred to.</param>
    /// <param name="events">The events that should have occurred.</param>
    /// <returns>A builder continuation.</returns>
    IThenStateBuilder Then(Guid id, params object[] events);
    /// <summary>
    /// Builds the test specification thus far.
    /// </summary>
    /// <returns>The test specification.</returns>
    EventCentricTestSpecification Build();
  }
}