using System;
using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;

namespace AggregateSource.Testing.Comparers
{
    [TestFixture]
    public class CompareNetObjectsBasedEventComparerTests
    {
        [Test]
        public void IsEventComparer()
        {
            var sut = new CompareNetObjectsBasedEventComparer(new CompareLogic());
            Assert.IsInstanceOf<IEventComparer>(sut);
        }

        [Test]
        public void CompareObjectsCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CompareNetObjectsBasedEventComparer(null));
        }

        [Test]
        public void CompareReturnsExpectedEventWhenObjectsDiffer()
        {
            var comparer = new CompareLogic();
            var sut = new CompareNetObjectsBasedEventComparer(comparer);

            var expected = new Event { Value = "1" };
            var actual = new Event { Value = "2" };
            var result = sut.Compare(expected, actual);

            Assert.That(result,
                Is.EquivalentTo(new[]
                {
                    new EventComparisonDifference(expected, actual, "Types [String,String], Item Expected.Value != Actual.Value, Values (1,2)")
                }).Using(EventComparisonDifferenceComparer.Instance));
        }

        [Test]
        public void CompareReturnsExpectedEventWhenObjectsAreEqual()
        {
            var comparer = new CompareLogic();
            var sut = new CompareNetObjectsBasedEventComparer(comparer);

            var expected = new Event { Value = "1" };
            var actual = new Event { Value = "1" };
            var result = sut.Compare(expected, actual);

            Assert.That(result, Is.Empty);
        }

        class Event
        {
            public string Value { get; set; }
        }

        class EventComparisonDifferenceComparer : IEqualityComparer<EventComparisonDifference>
        {
            public static readonly IEqualityComparer<EventComparisonDifference> Instance = new EventComparisonDifferenceComparer();

            public bool Equals(EventComparisonDifference x, EventComparisonDifference y)
            {
                return Equals(x.Expected, y.Expected) &&
                       Equals(x.Actual, y.Actual) &&
                       Equals(x.Message, y.Message);
            }

            public int GetHashCode(EventComparisonDifference obj)
            {
                return (obj.Expected != null ? obj.Expected.GetHashCode() : 0) ^
                       (obj.Actual != null ? obj.Actual.GetHashCode() : 0) ^
                       (obj.Message != null ? obj.Message.GetHashCode() : 0);
            }
        }
    }
}