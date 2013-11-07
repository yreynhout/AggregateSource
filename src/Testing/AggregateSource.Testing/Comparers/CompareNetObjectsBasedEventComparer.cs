#if !NET20
using System;
using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;

namespace AggregateSource.Testing.Comparers
{
    /// <summary>
    /// Compares events using a <see cref="ICompareObjects"/> object and reports the differences.
    /// </summary>
    public class CompareNetObjectsBasedEventComparer : IEventComparer
    {
        private readonly ICompareObjects _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareNetObjectsBasedEventComparer"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="comparer"/> is <c>null</c>.</exception>
        public CompareNetObjectsBasedEventComparer(ICompareObjects comparer)
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
        public IEnumerable<EventComparisonDifference> Compare(object expected, object actual)
        {
            if (!_comparer.Compare(expected, actual))
            {
                foreach (var difference in _comparer.Differences)
                {
                    yield return new EventComparisonDifference(
                        expected, 
                        actual,
                        difference.ToString());
                }
            }
        }
    }
}
#endif