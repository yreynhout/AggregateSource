using System;
using System.Collections;
using System.Collections.Generic;

namespace AggregateSource {
  /// <summary>
  /// Records events applied to an <see cref="IAggregateRootEntity"/> or <see cref="Entity"/>.
  /// </summary>
  public class EventRecorder : IEnumerable<object> {
    readonly List<object> _recorded;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventRecorder"/> class.
    /// </summary>
    public EventRecorder() {
      _recorded = new List<object>();
    }

    /// <summary>
    /// Records that the specified event happened.
    /// </summary>
    /// <param name="event">The event to record.</param>
    /// <exception cref="ArgumentNullException">Thrown when the specified <paramref name="@event"/> is <c>null</c>.</exception>
    public void Record(object @event) {
      if (@event == null) throw new ArgumentNullException("event");
      _recorded.Add(@event);
    }

    /// <summary>
    /// Resets this instance to its initial state.
    /// </summary>
    public void Reset() {
      _recorded.Clear();
    }

    public IEnumerator<object> GetEnumerator() {
      return _recorded.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}
