using System;

namespace AggregateSource.Testing.AggregateCentric
{
    /// <summary>
    /// Represents an result-centric test specification, meaning that the outcome revolves around the result of executing a query method.
    /// </summary>
    public class ResultCentricAggregateQueryTestSpecification
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;
        readonly Func<IAggregateRootEntity, object> _when;
        readonly object _then;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultCentricAggregateQueryTestSpecification"/> class.
        /// </summary>
        /// <param name="sutFactory">The sut factory.</param>
        /// <param name="givens">The events to arrange.</param>
        /// <param name="when">The query method to act upon.</param>
        /// <param name="then">The events to assert.</param>
        public ResultCentricAggregateQueryTestSpecification(Func<IAggregateRootEntity> sutFactory, object[] givens,
                                                            Func<IAggregateRootEntity, object> when, object then)
        {
            _sutFactory = sutFactory;
            _givens = givens;
            _when = when;
            _then = then;
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
        /// The expected result to assert.
        /// </summary>
        public object Then
        {
            get { return _then; }
        }
    }
}