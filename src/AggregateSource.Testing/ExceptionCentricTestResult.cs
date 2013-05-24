using System;

namespace AggregateSource.Testing {
  /// <summary>
  /// The result of an exception centric test specification.
  /// </summary>
  public class ExceptionCentricTestResult {
    readonly ExceptionCentricTestSpecification _specification;
    readonly TestResultState _state;
    readonly Optional<Exception> _actualException;
    readonly Optional<Tuple<string, object>[]> _actualEvents;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionCentricTestResult"/> class.
    /// </summary>
    /// <param name="specification">The specification.</param>
    /// <param name="state">The state.</param>
    /// <param name="actualException"></param>
    /// <param name="actualEvents"></param>
    internal ExceptionCentricTestResult(ExceptionCentricTestSpecification specification, TestResultState state, Exception actualException = null, Tuple<string, object>[] actualEvents = null) {
      _specification = specification;
      _state = state;
      _actualException = actualException == null ? Optional<Exception>.Empty : new Optional<Exception>(actualException);
      _actualEvents = actualEvents == null ? Optional<Tuple<string, object>[]>.Empty : new Optional<Tuple<string, object>[]>(actualEvents);
    }

    /// <summary>
    /// Gets the test specification associated with this result.
    /// </summary>
    /// <value>
    /// The test specification.
    /// </value>
    public ExceptionCentricTestSpecification Specification {
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
    /// Gets the exception that happened instead of the expected one, or empty if one didn't happen at all.
    /// </summary>
    /// <value>
    /// The exception.
    /// </value>
    public Optional<Exception> But {
      get { return _actualException; }
    }

    /// <summary>
    /// Gets the events that happened instead of the expected exception, or empty if none happened at all.
    /// </summary>
    /// <value>
    /// The events.
    /// </value>
    public Optional<Tuple<string, object>[]> Buts {
      get { return _actualEvents; }
    }
  }
}