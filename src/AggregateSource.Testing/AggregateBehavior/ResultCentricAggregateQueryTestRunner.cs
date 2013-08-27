using System;
using System.Linq;

namespace AggregateSource.Testing.AggregateBehavior
{
    /// <summary>
    /// Represents an aggregate query test specification runner.
    /// </summary>
    public class ResultCentricAggregateQueryTestRunner : IResultCentricAggregateQueryTestRunner
    {
        readonly IResultComparer _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultCentricAggregateQueryTestRunner"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use when comparing events.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="comparer"/> is <c>null</c>.</exception>
        public ResultCentricAggregateQueryTestRunner(IResultComparer comparer)
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
        public ResultCentricAggregateQueryTestResult Run(ResultCentricAggregateQueryTestSpecification specification)
        {
            if (specification == null) throw new ArgumentNullException("specification");
            var sut = specification.SutFactory();
            sut.Initialize(specification.Givens);
            object queryResult = null;
            var result = Catch.Exception(() => queryResult = specification.When(sut));
            if (result.HasValue)
            {
                return new ResultCentricAggregateQueryTestResult(specification, TestResultState.Failed, Optional<object>.Empty, new Optional<Exception>(result.Value), Optional<object[]>.Empty);
            }
            if (_comparer.Compare(queryResult, specification.Then).Any())
            {
                return new ResultCentricAggregateQueryTestResult(specification, TestResultState.Failed, new Optional<object>(queryResult), Optional<Exception>.Empty, Optional<object[]>.Empty);
            }
            if (sut.HasChanges())
            {
                return new ResultCentricAggregateQueryTestResult(specification, TestResultState.Failed, Optional<object>.Empty, Optional<Exception>.Empty, new Optional<object[]>(sut.GetChanges().ToArray()));
            }
            return new ResultCentricAggregateQueryTestResult(specification, TestResultState.Passed, Optional<object>.Empty, Optional<Exception>.Empty, Optional<object[]>.Empty);
        }
    }
}