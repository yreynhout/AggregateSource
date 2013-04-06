namespace AggregateSource.Testing {
  /// <summary>
  /// The given state within the test specification building process.
  /// </summary>
  public interface IGivenStateBuilder {
    /// <summary>
    /// Given the following events occured.
    /// </summary>
    /// <param name="identifier">The aggregate identifier the events are to be associated with.</param>
    /// <param name="events">The events that occurred.</param>
    /// <returns>A builder continuation.</returns>
    IGivenStateBuilder Given(string identifier, params object[] events);
    /// <summary>
    /// When a command occurs.
    /// </summary>
    /// <param name="message">The command message.</param>
    /// <returns>A builder continuation.</returns>
    IWhenStateBuilder When(object message);
  }
}