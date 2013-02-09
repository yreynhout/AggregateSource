using System;

namespace AggregateSource.Testing {
  /// <summary>
  /// The given-when-then test specification builder bootstrapper.
  /// </summary>
  public static class Scenario {
    /// <summary>
    /// Given the following events occured.
    /// </summary>
    /// <param name="id">The aggregate the events occured to.</param>
    /// <param name="events">The events that occurred.</param>
    /// <returns>A builder continuation.</returns>
    public static IGivenStateBuilder Given(Guid id, params object[] events) {
      return new TestSpecificationBuilder().Given(id, events);
    }

    /// <summary>
    /// When a command occurs.
    /// </summary>
    /// <param name="message">The command message.</param>
    /// <returns>A builder continuation.</returns>
    public static IWhenStateBuilder When(object message) {
      return new TestSpecificationBuilder().When(message);
    }
  }
}