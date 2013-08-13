using System;
using System.Collections.Generic;
using System.Linq;

namespace AggregateSource.Testing.AggregateBehavior
{
    /// <summary>
    /// Represents an aggregate factory test specification runner.
    /// </summary>
    public class EventCentricAggregateFactoryTestRunner : IEventCentricAggregateFactoryTestRunner
    {
        readonly IEqualityComparer<object> _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventCentricAggregateFactoryTestRunner"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use when comparing events.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="comparer"/> is <c>null</c>.</exception>
        public EventCentricAggregateFactoryTestRunner(IEqualityComparer<object> comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            _comparer = comparer;
        }

        /// <summary>
        /// Runs the specified test specification.
        /// </summary>
        /// <param name="specification">The test specification to run.</param>
        /// <returns>
        /// The result of running the test specification.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="specification"/> is <c>null</c>.</exception>
        public EventCentricAggregateFactoryTestResult Run(EventCentricAggregateFactoryTestSpecification specification)
        {
            if (specification == null) throw new ArgumentNullException("specification");
            var sut = specification.SutFactory();
            sut.Initialize(specification.Givens);
            IAggregateRootEntity factoryResult = null;
            var result = Catch.Exception(() => factoryResult = specification.When(sut));
            if (result.HasValue)
            {
                return new EventCentricAggregateFactoryTestResult(specification, TestResultState.Failed, actualException: result.Value);
            }
            var actualEvents = factoryResult.GetChanges().ToArray();
            if (!actualEvents.SequenceEqual(specification.Thens, _comparer))
            {
                return new EventCentricAggregateFactoryTestResult(specification, TestResultState.Failed, actualEvents);
            }
            return new EventCentricAggregateFactoryTestResult(specification, TestResultState.Passed);
        }
    }
}