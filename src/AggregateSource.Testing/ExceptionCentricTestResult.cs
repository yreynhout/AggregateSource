using System;

namespace AggregateSource.Testing {
  /// <summary>
  /// The result of an exception centric test specification.
  /// </summary>
  public class ExceptionCentricTestResult {
    readonly ExceptionCentricTestSpecification _specification;
    readonly TestResultState _state;
    readonly Optional<Exception> _actual;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionCentricTestResult"/> class.
    /// </summary>
    /// <param name="specification">The specification.</param>
    /// <param name="state">The state.</param>
    /// <param name="actual">The actual.</param>
    internal ExceptionCentricTestResult(ExceptionCentricTestSpecification specification, TestResultState state, Exception actual = null) {
      _specification = specification;
      _state = state;
      _actual = actual == null ? Optional<Exception>.Empty : new Optional<Exception>(actual);
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
      get { return _actual; }
    }
  }
}