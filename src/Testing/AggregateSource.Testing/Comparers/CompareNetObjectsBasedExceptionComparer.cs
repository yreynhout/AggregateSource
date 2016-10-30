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
        private readonly ICompareLogic _logic;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareNetObjectsBasedExceptionComparer"/> class.
        /// </summary>
        /// <param name="logic">The compare logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="logic"/> is <c>null</c>.</exception>
        public CompareNetObjectsBasedExceptionComparer(ICompareLogic logic)
        {
            if (logic == null) throw new ArgumentNullException("logic");
            _logic = logic;
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
            var result = _logic.Compare(expected, actual);
            if (!result.AreEqual)
            {
                foreach (var difference in result.Differences)
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