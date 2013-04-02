using System;

namespace AggregateSource.Testing {
  public class ExceptionCentricAggregateQueryTestSpecification {
    readonly Func<IAggregateRootEntity> _sutFactory;
    readonly object[] _givens;
    readonly Func<IAggregateRootEntity, object> _when;
    readonly Exception _throws;

    public ExceptionCentricAggregateQueryTestSpecification(Func<IAggregateRootEntity> sutFactory, object[] givens,
                                                           Func<IAggregateRootEntity, object> when, Exception throws) {
      _sutFactory = sutFactory;
      _givens = givens;
      _when = when;
      _throws = throws;
    }

    public Func<IAggregateRootEntity> SutFactory {
      get { return _sutFactory; }
    }

    public object[] Givens {
      get { return _givens; }
    }

    public Func<IAggregateRootEntity, object> When {
      get { return _when; }
    }

    public Exception Throws {
      get { return _throws; }
    }
  }
}