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
#if NET20
                    return specification.Fail(new List<object>(sut.GetChanges()).ToArray());
#else
                    return specification.Fail(sut.GetChanges().ToArray());
#endif
                }
                return specification.Fail(queryResult);
            }
            var actualException = result.Value;
#if NET20
            using (var enumerator = _comparer.Compare(actualException, specification.Throws).GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    return specification.Fail(actualException);
                }
            }
#else
            if (_comparer.Compare(actualException, specification.Throws).Any())
            {
                return specification.Fail(actualException);
            }
#endif
            if (sut.HasChanges())
            {
#if NET20
                return specification.Fail(new List<object>(sut.GetChanges()));
#else
                return specification.Fail(sut.GetChanges().ToArray());
#endif
            }return specification.Pass();
        }
    }
}