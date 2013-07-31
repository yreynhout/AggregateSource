using System;

namespace AggregateSource.Testing.AggregateCentric
{
    /// <summary>
    /// Represents an event centric test specification, meaning that the outcome revolves around events as the result of executing a factory method on an aggregate.
    /// </summary>
    public class EventCentricAggregateFactoryTestSpecification
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;
        readonly Func<IAggregateRootEntity, IAggregateRootEntity> _when;
        readonly object[] _thens;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventCentricAggregateFactoryTestSpecification"/> class.
        /// </summary>
        /// <param name="sutFactory">The sut factory.</param>
        /// <param name="givens">The events to arrange.</param>
        /// <param name="when">The factory method to act upon.</param>
        /// <param name="thens">The events to assert.</param>
        public EventCentricAggregateFactoryTestSpecification(Func<IAggregateRootEntity> sutFactory, object[] givens,
                                                             Func<IAggregateRootEntity, IAggregateRootEntity> when,
                                                             object[] thens)
        {
            _sutFactory = sutFactory;
            _givens = givens;
            _when = when;
            _thens = thens;
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
        /// The factory method to act upon.
        /// </summary>
        public Func<IAggregateRootEntity, IAggregateRootEntity> When
        {
            get { return _when; }
        }

        /// <summary>
        /// The expected events to assert.
        /// </summary>
        public object[] Thens
        {
            get { return _thens; }
        }
    }
}