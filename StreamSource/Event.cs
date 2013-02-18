using System;

namespace StreamSource {
  public class Event {
    /// <summary>
    /// Initializes a new instance of the <see cref="Event"/> class.
    /// </summary>
    /// <param name="headers">The headers.</param>
    /// <param name="message">The message.</param>
    /// <exception cref="System.ArgumentNullException">
    /// Thrown when the <paramref name="headers"/> or <paramref name="message"/> is null.
    /// </exception>
    public Event(Tuple<string, string>[] headers, object message) {
      if (headers == null) throw new ArgumentNullException("headers");
      if (message == null) throw new ArgumentNullException("message");
      Headers = headers;
      Message = message;
    }

    /// <summary>
    /// Gets the headers associated with the event.
    /// </summary>
    public Tuple<string, string>[] Headers { get; private set; }
    /// <summary>
    /// Gets the message that represents the event.
    /// </summary>
    public object Message { get; private set; }
  }
}