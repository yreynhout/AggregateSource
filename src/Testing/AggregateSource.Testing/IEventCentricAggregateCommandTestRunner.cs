using System;

namespace AggregateSource.Testing
{
    /// <summary>
    /// Represents an aggregate command test specification runner.
    /// </summary>
    public interface IEventCentricAggregateCommandTestRunner
    {
        /// <summary>
        /// Runs the specified test specification.
        /// </summary>
        /// <param name="specification">The test specification to run.</param>
        /// <returns>The result of running the test specification.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="specification"/> is <c>null</c>.</exception>
        EventCentricAggregateCommandTestResult Run(EventCentricAggregateCommandTestSpecification specification);
    }
}