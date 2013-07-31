using System;

namespace AggregateSource.Testing.AggregateCentric
{
    /// <summary>
    /// Represents an exception centric test specification, meaning that the outcome revolves around an exception as a result of executing a query method on an aggregate.
    /// </summary>
    public class ExceptionCentricAggregateQueryTestSpecification
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;
        readonly Func<IAggregateRootEntity, object> _when;
        readonly Exception _throws;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionCentricAggregateQueryTestSpecification"/> class.
        /// </summary>
        /// <param name="sutFactory">The sut factory.</param>
        /// <param name="givens">The events to arrange.</param>
        /// <param name="when">The query method to act upon.</param>
        /// <param name="throws">The expected exception to assert.</param>
        public ExceptionCentricAggregateQueryTestSpecification(Func<IAggregateRootEntity> sutFactory, object[] givens,
                                                               Func<IAggregateRootEntity, object> when, Exception throws)
        {
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
        public Func<IAggregateRootEntity> SutFactory
        {
            get { return _sutFactory; }
        }

        /// <summary>
        /// The events to arrange.
        /// </summary>
        public object[] Givens
        {
            get { return _givens; }
        }

        /// <summary>
        /// The query method to act upon.
        /// </summary>
        public Func<IAggregateRootEntity, object> When
        {
            get { return _when; }
        }

        /// <summary>
        /// The expected exception to assert.
        /// </summary>
        public Exception Throws
        {
            get { return _throws; }
        }
    }
}