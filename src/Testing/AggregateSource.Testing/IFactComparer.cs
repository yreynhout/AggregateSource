using System.Collections.Generic;

namespace AggregateSource.Testing
{
    /// <summary>
    /// Contract to compare if the expected and actual fact are equal.
    /// </summary>
    public interface IFactComparer
    {
        /// <summary>
        /// Compares the expected to the actual fact.
        /// </summary>
        /// <param name="expected">The expected fact.</param>
        /// <param name="actual">The actual fact.</param>
        /// <returns>An enumeration of <see cref="FactComparisonDifference">differences</see>, or empty if none found.</returns>
        IEnumerable<FactComparisonDifference> Compare(Fact expected, Fact actual);
    }
}