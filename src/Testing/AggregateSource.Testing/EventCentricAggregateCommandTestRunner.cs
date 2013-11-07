using System;
#if NET20
using System.Collections.Generic;
#endif
#if !NET20
using System.Linq;
#endif

namespace AggregateSource.Testing
{
    /// <summary>
    /// Represents an aggregate command test specification runner.
    /// </summary>
    public class EventCentricAggregateCommandTestRunner : IEventCentricAggregateCommandTestRunner
    {
        readonly IEventComparer _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventCentricAggregateCommandTestRunner"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use when comparing events.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="comparer"/> is <c>null</c>.</exception>
        public EventCentricAggregateCommandTestRunner(IEventComparer comparer)
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
        public EventCentricAggregateCommandTestResult Run(EventCentricAggregateCommandTestSpecification specification)
        {
            if (specification == null) throw new ArgumentNullException("specification");
            var sut = specification.SutFactory();
            sut.Initialize(specification.Givens);
            var result = Catch.Exception(() => specification.When(sut));
            if (result.HasValue)
            {
                return specification.Fail(result.Value);
            }
#if NET20
            var actualEvents = new List<object>(sut.GetChanges()).ToArray();
#else
            var actualEvents = sut.GetChanges().ToArray();
#endif
            if (!actualEvents.SequenceEqual(specification.Thens, new WrappedEventComparerEqualityComparer(_comparer)))
            {
                return specification.Fail(actualEvents);
            }
            return specification.Pass();
        }
    }
}