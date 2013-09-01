using System;
using System.Linq;

namespace AggregateSource.Testing.AggregateBehavior
{
    /// <summary>
    /// Represents an aggregate constructor test specification runner.
    /// </summary>
    public class ExceptionCentricAggregateConstructorTestRunner : IExceptionCentricAggregateConstructorTestRunner
    {
        readonly IExceptionComparer _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionCentricAggregateConstructorTestRunner"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use when comparing exceptions.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="comparer"/> is <c>null</c>.</exception>
        public ExceptionCentricAggregateConstructorTestRunner(IExceptionComparer comparer)
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
        public ExceptionCentricAggregateConstructorTestResult Run(ExceptionCentricAggregateConstructorTestSpecification specification)
        {
            if (specification == null) throw new ArgumentNullException("specification");
            IAggregateRootEntity sut = null;
            var result = Catch.Exception(() => sut = specification.SutFactory());
            if (!result.HasValue)
            {
                if (sut.HasChanges())
                {
                    return new ExceptionCentricAggregateConstructorTestResult(
                        specification,
                        TestResultState.Failed,
                        Optional<Exception>.Empty,
                        new Optional<object[]>(sut.GetChanges().ToArray()));
                }
                return new ExceptionCentricAggregateConstructorTestResult(
                    specification,
                    TestResultState.Failed,
                    Optional<Exception>.Empty,
                    Optional<object[]>.Empty);
            }
            var actualException = result.Value;
            if (_comparer.Compare(actualException, specification.Throws).Any())
            {
                return new ExceptionCentricAggregateConstructorTestResult(
                    specification,
                    TestResultState.Failed,
                    new Optional<Exception>(actualException),
                    Optional<object[]>.Empty);
            }
            return new ExceptionCentricAggregateConstructorTestResult(
                specification,
                TestResultState.Passed,
                    Optional<Exception>.Empty,
                    Optional<object[]>.Empty);
        }
    }
}