using System;
using System.Collections.Generic;
using System.Linq;

namespace AggregateSource.Testing
{
    /// <summary>
    /// Represents an aggregate constructor test specification runner.
    /// </summary>
    public class EventCentricAggregateConstructorTestRunner : IEventCentricAggregateConstructorTestRunner
    {
        readonly IEqualityComparer<object> _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventCentricAggregateConstructorTestRunner"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use when comparing events.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="comparer"/> is <c>null</c>.</exception>
        public EventCentricAggregateConstructorTestRunner(IEqualityComparer<object> comparer)
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
        public EventCentricAggregateConstructorTestResult Run(EventCentricAggregateConstructorTestSpecification specification)
        {
            if (specification == null) throw new ArgumentNullException("specification");
            IAggregateRootEntity sut = null;
            var result = Catch.Exception(() => sut = specification.SutFactory());
            if (result.HasValue)
            {
                return new EventCentricAggregateConstructorTestResult(specification, TestResultState.Failed, actualException: result.Value);
            }
            var actualEvents = sut.GetChanges().ToArray();
            if (!actualEvents.SequenceEqual(specification.Thens, _comparer))
            {
                return new EventCentricAggregateConstructorTestResult(specification, TestResultState.Failed, actualEvents);
            }
            return new EventCentricAggregateConstructorTestResult(specification, TestResultState.Passed);
        }
    }
}