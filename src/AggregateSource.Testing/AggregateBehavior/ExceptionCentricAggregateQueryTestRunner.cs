using System;
using System.Collections.Generic;
using System.Linq;

namespace AggregateSource.Testing.AggregateBehavior
{
    /// <summary>
    /// Represents an aggregate constructor test specification runner.
    /// </summary>
    public class ExceptionCentricAggregateQueryTestRunner : IExceptionCentricAggregateQueryTestRunner
    {
        readonly IExceptionComparer _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionCentricAggregateQueryTestRunner"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use when comparing exceptions.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="comparer"/> is <c>null</c>.</exception>
        public ExceptionCentricAggregateQueryTestRunner(IExceptionComparer comparer)
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
        public ExceptionCentricAggregateQueryTestResult Run(ExceptionCentricAggregateQueryTestSpecification specification)
        {
            if (specification == null) throw new ArgumentNullException("specification");
            var sut = specification.SutFactory();
            sut.Initialize(specification.Givens);
            object queryResult = null;
            var result = Catch.Exception(() => queryResult = specification.When(sut));
            if (!result.HasValue)
            {
                if (sut.HasChanges())
                {
                    return new ExceptionCentricAggregateQueryTestResult(specification, TestResultState.Failed, Optional<Exception>.Empty, new Optional<object[]>(sut.GetChanges().ToArray()), Optional<object>.Empty);
                }
                return new ExceptionCentricAggregateQueryTestResult(specification, TestResultState.Failed, Optional<Exception>.Empty, Optional<object[]>.Empty, new Optional<object>(queryResult));
            }
            var actualException = result.Value;
            if (_comparer.Compare(actualException, specification.Throws).Any())
            {
                return new ExceptionCentricAggregateQueryTestResult(specification, TestResultState.Failed, new Optional<Exception>(actualException), Optional<object[]>.Empty, Optional<object>.Empty);
            }
            if (sut.HasChanges())
            {
                return new ExceptionCentricAggregateQueryTestResult(specification, TestResultState.Failed, Optional<Exception>.Empty, new Optional<object[]>(sut.GetChanges().ToArray()), Optional<object>.Empty);
            }
            return new ExceptionCentricAggregateQueryTestResult(specification, TestResultState.Passed, Optional<Exception>.Empty, Optional<object[]>.Empty, Optional<object>.Empty);
        }
    }
}