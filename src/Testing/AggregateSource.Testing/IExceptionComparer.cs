using System;
using System.Collections.Generic;

namespace AggregateSource.Testing
{
    /// <summary>
    /// Contract to compare if the expected and actual exception are equal.
    /// </summary>
    public interface IExceptionComparer
    {
        /// <summary>
        /// Compares the expected to the actual exception.
        /// </summary>
        /// <param name="expected">The expected exception.</param>
        /// <param name="actual">The actual exception.</param>
        /// <returns>An enumeration of <see cref="ExceptionComparisonDifference">differences</see>, or empty if none found.</returns>
        IEnumerable<ExceptionComparisonDifference> Compare(Exception expected, Exception actual);
    }
}