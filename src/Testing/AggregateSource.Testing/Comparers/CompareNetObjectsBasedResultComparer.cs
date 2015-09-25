#if !NET20
using System;
using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;

namespace AggregateSource.Testing.Comparers
{
    /// <summary>
    /// Compares results using a <see cref="ICompareLogic"/> object and reports the differences.
    /// </summary>
    public class CompareNetObjectsBasedResultComparer : IResultComparer
    {
        private readonly ICompareLogic _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareNetObjectsBasedResultComparer"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="comparer"/> is <c>null</c>.</exception>
        public CompareNetObjectsBasedResultComparer(ICompareLogic comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            _comparer = comparer;
        }

        /// <summary>
        /// Compares the expected to the actual event.
        /// </summary>
        /// <param name="expected">The expected event.</param>
        /// <param name="actual">The actual event.</param>
        /// <returns>
        /// An enumeration of <see cref="EventComparisonDifference">differences</see>, or empty if none found.
        /// </returns>
        public IEnumerable<ResultComparisonDifference> Compare(object expected, object actual)
        {
            var compareResult = _comparer.Compare(expected, actual);
            if (!compareResult.AreEqual)
            {
                foreach (var difference in compareResult.Differences)
                {
                    yield return new ResultComparisonDifference(
                        expected, 
                        actual,
                        difference.ToString());
                }
            }
        }
    }
}
#endif