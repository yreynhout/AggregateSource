using System;

namespace AggregateSource.Testing {
  /// <summary>
  /// The result of an event centric test specification.
  /// </summary>
  public class EventCentricTestResult {
    readonly EventCentricTestSpecification _specification;
    readonly TestResultState _state;
    readonly Optional<Tuple<string, object>[]> _actual;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventCentricTestResult"/> class.
    /// </summary>
    /// <param name="specification">The specification.</param>
    /// <param name="state">The state.</param>
    /// <param name="actual">The actual.</param>
    internal EventCentricTestResult(EventCentricTestSpecification specification, TestResultState state, Tuple<string, object>[] actual = null) {
      _specification = specification;
      _state = state;
      _actual = actual == null ? Optional<Tuple<string, object>[]>.Empty : new Optional<Tuple<string, object>[]>(actual);
    }

    /// <summary>
    /// Gets the test specification associated with this result.
    /// </summary>
    /// <value>
    /// The test specification.
    /// </value>
    public EventCentricTestSpecification Specification {
      get { return _specification; }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="EventCentricTestResult"/> has passed.
    /// </summary>
    /// <value>
    ///   <c>true</c> if passed; otherwise, <c>false</c>.
    /// </value>
    public bool Passed {
      get { return _state == TestResultState.Passed; }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="EventCentricTestResult"/> has failed.
    /// </summary>
    /// <value>
    ///   <c>true</c> if failed; otherwise, <c>false</c>.
    /// </value>
    public bool Failed {
      get { return _state == TestResultState.Failed; }
    }

    /// <summary>
    /// Gets the events that happened instead of the expected ones, or empty if none happened at all.
    /// </summary>
    /// <value>
    /// The events.
    /// </value>
    public Optional<Tuple<string, object>[]> Buts {
      get { return _actual; }
    }
  }
}