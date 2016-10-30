using System;
using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;

namespace AggregateSource.Testing.Comparers
{
    [TestFixture]
    public class CompareNetObjectsBasedFactComparerTests
    {
        [Test]
        public void IsFactComparer()
        {
            var sut = new CompareNetObjectsBasedFactComparer(new CompareLogic());
            Assert.IsInstanceOf<IFactComparer>(sut);
        }

        [Test]
        public void CompareObjectsCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CompareNetObjectsBasedFactComparer(null));
        }

        [Test]
        public void CompareReturnsExpectedFactWhenIdentifiersDiffer()
        {
            var comparer = new CompareLogic();
            var sut = new CompareNetObjectsBasedFactComparer(comparer);

            var @event = new Event { Value = "1" };
            var expected = new Fact("123", @event);
            var actual = new Fact("456", @event);
            var result = sut.Compare(expected, actual);

            Assert.That(result,
                Is.EquivalentTo(new[]
                {
                    new FactComparisonDifference(expected, actual, "Expected.Identifier != Actual.Identifier (123,456)")
                }).Using(FactComparisonDifferenceComparer.Instance));
        }

        [Test]
        public void CompareReturnsExpectedFactWhenEventsDiffer()
        {
            var comparer = new CompareLogic();
            var sut = new CompareNetObjectsBasedFactComparer(comparer);

            var expected = new Fact("123", new Event { Value = "1" });
            var actual = new Fact("123", new Event { Value = "2" });
            var result = sut.Compare(expected, actual);

            Assert.That(result,
                Is.EquivalentTo(new[]
                {
                    new FactComparisonDifference(expected, actual, "Types [String,String], Item Expected.Value != Actual.Value, Values (1,2)")
                }).Using(FactComparisonDifferenceComparer.Instance));
        }

        class Event
        {
            public string Value { get; set; }
        }

        class FactComparisonDifferenceComparer : IEqualityComparer<FactComparisonDifference>
        {
            public static readonly IEqualityComparer<FactComparisonDifference> Instance = new FactComparisonDifferenceComparer();

            public bool Equals(FactComparisonDifference x, FactComparisonDifference y)
            {
                return Equals(x.Expected, y.Expected) &&
                       Equals(x.Actual, y.Actual) &&
                       Equals(x.Message, y.Message);
            }

            public int GetHashCode(FactComparisonDifference obj)
            {
                return obj.Expected.GetHashCode() ^
                       obj.Actual.GetHashCode() ^
                       (obj.Message != null ? obj.Message.GetHashCode() : 0);
            }
        }
    }
}