#if !NET20
using System;
using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;

namespace AggregateSource.Testing.Comparers
{
    /// <summary>
    /// Compares exception using a <see cref="ICompareLogic"/> object and reports the differences.
    /// </summary>
    public class CompareNetObjectsBasedExceptionComparer : IExceptionComparer
    {
        private readonly ICompareLogic _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareNetObjectsBasedExceptionComparer"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="comparer"/> is <c>null</c>.</exception>
        public CompareNetObjectsBasedExceptionComparer(ICompareLogic comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            _comparer = comparer;
        }

        /// <summary>
        /// Compares the expected to the actual exception.
        /// </summary>
        /// <param name="expected">The expected exception.</param>
        /// <param name="actual">The actual exception.</param>
        /// <returns>
        /// An enumeration of <see cref="ExceptionComparisonDifference">differences</see>, or empty if none found.
        /// </returns>
        public IEnumerable<ExceptionComparisonDifference> Compare(Exception expected, Exception actual)
        {
            var compareResult = _comparer.Compare(expected, actual);
            if (!compareResult.AreEqual)
            {
                foreach (var difference in compareResult.Differences)
                {
                    yield return new ExceptionComparisonDifference(
                        expected, 
                        actual,
                        difference.ToString());
                }
            }
        }
    }
}
#endif