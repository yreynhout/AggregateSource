using System;

namespace AggregateSource.Testing {
  /// <summary>
  /// Represents a test specification.
  /// </summary>
  public class TestSpecification {
    readonly Tuple<Guid, object>[] _givens;
    readonly object _when;
    readonly Tuple<Guid, object>[] _thens;
    readonly Exception _throws;

    /// <summary>
    /// Initializes a new <see cref="TestSpecification"/> instance.
    /// </summary>
    /// <param name="givens">The specification givens.</param>
    /// <param name="when">The specification when.</param>
    /// <param name="thens">The specification thens.</param>
    /// <param name="throws">The specification exception thrown.</param>
    public TestSpecification(Tuple<Guid, object>[] givens, object when, Tuple<Guid, object>[] thens, Exception throws) {
      _givens = givens;
      _thens = thens;
      _when = when;
      _throws = throws;
    }

    /// <summary>
    /// The givens.
    /// </summary>
    public Tuple<Guid, object>[] Givens {
      get { return _givens; }
    }

    /// <summary>
    /// The when
    /// </summary>
    public object When {
      get { return _when; }
    }

    /// <summary>
    /// The thens.
    /// </summary>
    public Tuple<Guid, object>[] Thens {
      get { return _thens; }
    }

    /// <summary>
    /// The thrown exception.
    /// </summary>
    public Exception Throws {
      get { return _throws; }
    }

    protected bool Equals(TestSpecification other) {
      return Equals(_givens, other._givens) && Equals(_when, other._when) && Equals(_thens, other._thens) && Equals(_throws, other._throws);
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != GetType()) return false;
      return Equals((TestSpecification)obj);
    }

    public override int GetHashCode() {
      return
        (_givens != null ? _givens.GetHashCode() : 0) ^
        (_when != null ? _when.GetHashCode() : 0) ^
        (_thens != null ? _thens.GetHashCode() : 0) ^
        (_throws != null ? _throws.GetHashCode() : 0);
    }
  }
}