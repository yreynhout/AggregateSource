using System;

namespace AggregateSource.Testing {
  /// <summary>
  /// The given-when-then test specification builder bootstrapper.
  /// </summary>
  public class Scenario : IGivenStateBuilder {
    /// <summary>
    /// Given the following events occured.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="events">The events that occurred.</param>
    /// <returns>A builder continuation.</returns>
    public IGivenStateBuilder Given(string identifier, params object[] events) {
      if (identifier == null) throw new ArgumentNullException("identifier");
      if (events == null) throw new ArgumentNullException("events");
      return new TestSpecificationBuilder().Given(identifier, events);
    }

    /// <summary>
    /// When a command occurs.
    /// </summary>
    /// <param name="message">The command message.</param>
    /// <returns>A builder continuation.</returns>
    public IWhenStateBuilder When(object message) {
      if (message == null) throw new ArgumentNullException("message");
      return new TestSpecificationBuilder().When(message);
    }
  }
}