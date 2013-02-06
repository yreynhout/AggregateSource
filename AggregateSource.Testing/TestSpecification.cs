using System;

namespace AggregateSource.Testing {
  public class TestSpecification {
    readonly Tuple<Guid, object>[] _givens;
    readonly object _when;
    readonly Tuple<Guid, object>[] _thens;
    readonly Exception _throws;

    public TestSpecification(Tuple<Guid, object>[] givens, object when, Tuple<Guid, object>[] thens, Exception throws) {
      _givens = givens;
      _thens = thens;
      _when = when;
      _throws = throws;
    }

    public Tuple<Guid, object>[] Givens {
      get { return _givens; }
    }

    public object When {
      get { return _when; }
    }

    public Tuple<Guid, object>[] Thens {
      get { return _thens; }
    }

    public Exception Throws {
      get { return _throws; }
    }
  }
}