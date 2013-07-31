using System;
using System.Collections.Generic;
using System.Linq;

namespace AggregateSource.Testing.AggregateCentric
{
    /// <summary>
    /// Represents an aggregate command test specification runner.
    /// </summary>
    public class ExceptionCentricAggregateFactoryTestRunner : IExceptionCentricAggregateFactoryTestRunner
    {
        readonly IEqualityComparer<Exception> _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionCentricAggregateFactoryTestRunner"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use when comparing exceptions.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="comparer"/> is <c>null</c>.</exception>
        public ExceptionCentricAggregateFactoryTestRunner(IEqualityComparer<Exception> comparer)
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
        public ExceptionCentricAggregateFactoryTestResult Run(ExceptionCentricAggregateFactoryTestSpecification specification)
        {
            if (specification == null) throw new ArgumentNullException("specification");
            var sut = specification.SutFactory();
            sut.Initialize(specification.Givens);
            IAggregateRootEntity factoryResult = null;
            var result = Catch.Exception(() => factoryResult = specification.When(sut));
            if (!result.HasValue)
            {
                if (factoryResult.HasChanges())
                {
                    return new ExceptionCentricAggregateFactoryTestResult(specification, TestResultState.Failed, actualEvents: factoryResult.GetChanges().ToArray());
                }
                return new ExceptionCentricAggregateFactoryTestResult(specification, TestResultState.Failed);
            }
            var actualException = result.Value;
            if (!_comparer.Equals(actualException, specification.Throws))
            {
                return new ExceptionCentricAggregateFactoryTestResult(specification, TestResultState.Failed, actualException);
            }
            return new ExceptionCentricAggregateFactoryTestResult(specification, TestResultState.Passed);
        }
    }
}