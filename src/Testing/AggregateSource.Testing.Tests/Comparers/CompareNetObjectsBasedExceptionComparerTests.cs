using System;
using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;

namespace AggregateSource.Testing.Comparers
{
    [TestFixture]
    public class CompareNetObjectsBasedExceptionComparerTests
    {
        [Test]
        public void IsExceptionComparer()
        {
            var sut = new CompareNetObjectsBasedExceptionComparer(new CompareLogic());
            Assert.IsInstanceOf<IExceptionComparer>(sut);
        }

        [Test]
        public void CompareObjectsCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CompareNetObjectsBasedExceptionComparer(null));
        }

        [Test]
        public void CompareReturnsExpectedExceptionWhenObjectsDiffer()
        {
            var comparer = new CompareLogic();
            var sut = new CompareNetObjectsBasedExceptionComparer(comparer);

            var expected = new Exception("1");
            var actual = new Exception("2");
            var result = sut.Compare(expected, actual);

            Assert.That(result,
                Is.EquivalentTo(new[]
                {
                    new ExceptionComparisonDifference(expected, actual, "Types [String,String], Item Expected.Message != Actual.Message, Values (1,2)")
                }).Using(ExceptionComparisonDifferenceComparer.Instance));
        }

        [Test]
        public void CompareReturnsExpectedExceptionWhenObjectsAreEqual()
        {
            var comparer = new CompareLogic();
            var sut = new CompareNetObjectsBasedExceptionComparer(comparer);

            var expected = new Exception("1");
            var actual = new Exception("1");
            var result = sut.Compare(expected, actual);

            Assert.That(result, Is.Empty);
        }

        class ExceptionComparisonDifferenceComparer : IEqualityComparer<ExceptionComparisonDifference>
        {
            public static readonly IEqualityComparer<ExceptionComparisonDifference> Instance = new ExceptionComparisonDifferenceComparer();

            public bool Equals(ExceptionComparisonDifference x, ExceptionComparisonDifference y)
            {
                return Equals(x.Expected, y.Expected) &&
                       Equals(x.Actual, y.Actual) &&
                       Equals(x.Message, y.Message);
            }

            public int GetHashCode(ExceptionComparisonDifference obj)
            {
                return (obj.Expected != null ? obj.Expected.GetHashCode() : 0) ^
                       (obj.Actual != null ? obj.Actual.GetHashCode() : 0) ^
                       (obj.Message != null ? obj.Message.GetHashCode() : 0);
            }
        }
    }
}