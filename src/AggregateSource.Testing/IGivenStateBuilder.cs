using System;

namespace AggregateSource.Testing {
  /// <summary>
  /// The given state within the test specification building process.
  /// </summary>
  public interface IGivenStateBuilder {
    /// <summary>
    /// Given the following events occured.
    /// </summary>
    /// <param name="id">The aggregate the events occured to.</param>
    /// <param name="events">The events that occurred.</param>
    /// <returns>A builder continuation.</returns>
    IGivenStateBuilder Given(Guid id, params object[] events);
    /// <summary>
    /// When a command occurs.
    /// </summary>
    /// <param name="message">The command message.</param>
    /// <returns>A builder continuation.</returns>
    IWhenStateBuilder When(object message);
  }
}