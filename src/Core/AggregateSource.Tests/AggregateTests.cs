using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource
{
    [TestFixture]
    public class AggregateTests
    {
        [Test, Combinatorial]
        public void UsingConstructorWithPartitionReturnsInstanceWithExpectedProperties(
            [ValueSource(typeof (AggregateTestsValueSource), "IdSource")] string identifier,
            [Values(Int32.MinValue, -1, 0, 1, Int32.MaxValue)] int version)
        {
            var root = new AggregateRootEntityStub();
            var sut = new Aggregate(identifier, version, root);

            Assert.That(sut.Identifier, Is.EqualTo(identifier));
            Assert.That(sut.ExpectedVersion, Is.EqualTo(version));
            Assert.That(sut.Root, Is.SameAs(root));
        }

        [Test, Combinatorial]
        public void UsingConstructorWithoutPartitionReturnsInstanceWithExpectedProperties(
            [ValueSource(typeof(AggregateTestsValueSource), "IdSource")] string identifier,
            [Values(Int32.MinValue, -1, 0, 1, Int32.MaxValue)] int version)
        {
            var root = new AggregateRootEntityStub();
            var sut = new Aggregate(identifier, version, root);

            Assert.That(sut.Identifier, Is.EqualTo(identifier));
            Assert.That(sut.ExpectedVersion, Is.EqualTo(version));
            Assert.That(sut.Root, Is.SameAs(root));
        }

        [Test]
        public void IdentifierCannotBeNull()
        {
            Assert.
                Throws<ArgumentNullException>(
                    () => new Aggregate(null, 0, new AggregateRootEntityStub()));
        }

        [Test]
        public void RootCannotBeNull()
        {
            Assert.
                Throws<ArgumentNullException>(
                    () => new Aggregate(Guid.NewGuid().ToString(), 0, null));
        }

        [Test]
        public void ToBuilderReturnsExpectedResult()
        {
            const string identifier = "identifier";
            const int expectedVersion = 123;
            var root = new AggregateRootEntityStub();
            var sut = new Aggregate(identifier, expectedVersion, root);

            var result = sut.ToBuilder();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Identifier, Is.EqualTo(identifier));
            Assert.That(result.ExpectedVersion, Is.EqualTo(expectedVersion));
            Assert.That(result.Root, Is.SameAs(root));
        }

        static class AggregateTestsValueSource
        {
            public static IEnumerable<string> IdSource
            {
                get
                {
                    yield return Guid.Empty.ToString();
                    yield return Guid.NewGuid().ToString();
                    yield return "Aggregate/" + Guid.Empty;
                    yield return "Aggregate/" + Guid.NewGuid();
                }
            }
        }
    }
}