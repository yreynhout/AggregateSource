using System;
using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;

namespace AggregateSource.Testing.Comparers
{
    /// <summary>
    /// Compares facts using a <see cref="ICompareObjects"/> object and reports the differences.
    /// </summary>
    public class CompareNetObjectsBasedFactComparer : IFactComparer
    {
        private readonly ICompareObjects _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareNetObjectsBasedFactComparer"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="comparer">comparer</paramref> is <c>null</c>.</exception>
        public CompareNetObjectsBasedFactComparer(ICompareObjects comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            _comparer = comparer;
        }

        /// <summary>
        /// Compares the expected to the actual fact.
        /// </summary>
        /// <param name="expected">The expected fact.</param>
        /// <param name="actual">The actual fact.</param>
        /// <returns>
        /// An enumeration of <see cref="FactComparisonDifference">differences</see>, or empty if none found.
        /// </returns>
        public IEnumerable<FactComparisonDifference> Compare(Fact expected, Fact actual)
        {
            if (string.CompareOrdinal(expected.Identifier, actual.Identifier) != 0)
            {
                yield return new FactComparisonDifference(
                    expected,
                    actual,
                    string.Format("Expected.Identifier != Actual.Identifier ({0},{1})", expected.Identifier, actual.Identifier));
            }

            if (!_comparer.Compare(expected.Event, actual.Event))
            {
                foreach (var difference in _comparer.Differences)
                {
                    yield return new FactComparisonDifference(
                        expected,
                        actual,
                        difference.ToString());
                }
            }
        }
    }
}