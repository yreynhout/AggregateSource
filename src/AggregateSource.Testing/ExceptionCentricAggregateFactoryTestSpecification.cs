using System;

namespace AggregateSource.Testing {
  /// <summary>
  /// Represents an exception centric test specification, meaning that the outcome revolves around an exception as a result of executing a factory method.
  /// </summary>
  public class ExceptionCentricAggregateFactoryTestSpecification {
    readonly Func<IAggregateRootEntity> _sutFactory;
    readonly object[] _givens;
    readonly Func<IAggregateRootEntity, IAggregateRootEntity> _when;
    readonly Exception _throws;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionCentricAggregateFactoryTestSpecification"/> class.
    /// </summary>
    /// <param name="sutFactory">The sut factory.</param>
    /// <param name="givens">The events to arrange.</param>
    /// <param name="when">The factory method to act upon.</param>
    /// <param name="throws">The expected exception to assert.</param>
    public ExceptionCentricAggregateFactoryTestSpecification(Func<IAggregateRootEntity> sutFactory, object[] givens,
                                                             Func<IAggregateRootEntity, IAggregateRootEntity> when,
                                                             Exception throws) {
      _sutFactory = sutFactory;
      _givens = givens;
      _when = when;
      _throws = throws;
    }

    /// <summary>
    /// Gets the sut factory.
    /// </summary>
    /// <value>
    /// The sut factory.
    /// </value>
    public Func<IAggregateRootEntity> SutFactory {
      get { return _sutFactory; }
    }

    /// <summary>
    /// The events to arrange.
    /// </summary>
    public object[] Givens {
      get { return _givens; }
    }

    /// <summary>
    /// The factory method to act upon.
    /// </summary>
    public Func<IAggregateRootEntity, IAggregateRootEntity> When {
      get { return _when; }
    }

    /// <summary>
    /// The expected exception to assert.
    /// </summary>
    public Exception Throws {
      get { return _throws; }
    }
  }
}