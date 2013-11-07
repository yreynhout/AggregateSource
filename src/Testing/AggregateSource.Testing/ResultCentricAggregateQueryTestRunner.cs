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
                return specification.Fail(result.Value);
            }
#if NET20
            using (var enumerator = _comparer.Compare(queryResult, specification.Then).GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    return specification.Fail(queryResult);
                }
            }
#else
            if (_comparer.Compare(queryResult, specification.Then).Any())
            {
                return specification.Fail(queryResult);
            }
#endif
            if (sut.HasChanges())
            {
#if NET20
                return specification.Fail(new List<object>(sut.GetChanges()));
#else
                return specification.Fail(sut.GetChanges().ToArray());
#endif
            }
            return specification.Pass();
        }
    }
}