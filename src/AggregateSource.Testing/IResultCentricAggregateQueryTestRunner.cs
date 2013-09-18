using System;

namespace AggregateSource.Testing
{
    /// <summary>
    /// Represents an aggregate query test specification runner.
    /// </summary>
    public interface IResultCentricAggregateQueryTestRunner
    {
        /// <summary>
        /// Runs the specified test specification.
        /// </summary>
        /// <param name="specification">The test specification to run.</param>
        /// <returns>The result of running the test specification.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="specification"/> is <c>null</c>.</exception>
        ResultCentricAggregateQueryTestResult Run(ResultCentricAggregateQueryTestSpecification specification);
    }
}