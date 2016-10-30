using System;
using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;

namespace AggregateSource.Testing.Comparers
{
    [TestFixture]
    public class CompareNetObjectsBasedResultComparerTests
    {
        [Test]
        public void IsResultComparer()
        {
            var sut = new CompareNetObjectsBasedResultComparer(new CompareLogic());
            Assert.IsInstanceOf<IResultComparer>(sut);
        }

        [Test]
        public void CompareObjectsCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CompareNetObjectsBasedResultComparer(null));
        }

        [Test]
        public void CompareReturnsExpectedResultWhenObjectsDiffer()
        {
            var comparer = new CompareLogic();
            var sut = new CompareNetObjectsBasedResultComparer(comparer);

            const int expected = 1;
            const int actual = 2;
            var result = sut.Compare(expected, actual);

            Assert.That(result,
                Is.EquivalentTo(new[]
                {
                    new ResultComparisonDifference(expected, actual, "Types [Int32,Int32], Item Expected != Actual, Values (1,2)")
                }).Using(ResultComparisonDifferenceComparer.Instance));
        }

        [Test]
        public void CompareReturnsExpectedResultWhenObjectsAreEqual()
        {
            var comparer = new CompareLogic();
            var sut = new CompareNetObjectsBasedResultComparer(comparer);

            const int expected = 1;
            const int actual = 1;
            var result = sut.Compare(expected, actual);

            Assert.That(result, Is.Empty);
        }

        class ResultComparisonDifferenceComparer : IEqualityComparer<ResultComparisonDifference>
        {
            public static readonly IEqualityComparer<ResultComparisonDifference> Instance = new ResultComparisonDifferenceComparer();

            public bool Equals(ResultComparisonDifference x, ResultComparisonDifference y)
            {
                return Equals(x.Expected, y.Expected) &&
                       Equals(x.Actual, y.Actual) &&
                       Equals(x.Message, y.Message);
            }

            public int GetHashCode(ResultComparisonDifference obj)
            {
                return (obj.Expected != null ? obj.Expected.GetHashCode() : 0) ^
                       (obj.Actual != null ? obj.Actual.GetHashCode() : 0) ^
                       (obj.Message != null ? obj.Message.GetHashCode() : 0);
            }
        }
    }
}
