using System;

namespace StreamSource {
  public class EventStreamChange {
    /// <summary>
    /// Initializes a new instance of the <see cref="EventStreamChange"/> class.
    /// </summary>
    /// <param name="streamId">The stream id.</param>
    /// <param name="expectedVersion">The expected version.</param>
    /// <param name="events">The events.</param>
    /// <exception cref="System.ArgumentNullException">events</exception>
    public EventStreamChange(string streamId, long expectedVersion, Event[] events) {
      if (events == null) throw new ArgumentNullException("events");
      StreamId = streamId;
      ExpectedVersion = expectedVersion;
      Events = events;
    }

    /// <summary>
    /// Gets the identifier of the affected stream.
    /// </summary>
    public string StreamId { get; private set; }
    /// <summary>
    /// Gets the version the affected stream is expected to be at.
    /// </summary>
    public long ExpectedVersion { get; private set; }
    /// <summary>
    /// Gets the events that should be appended to the affected stream.
    /// </summary>
    public Event[] Events { get; private set; }
  }
}