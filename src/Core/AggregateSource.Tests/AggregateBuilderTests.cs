using System;
using NUnit.Framework;

namespace AggregateSource
{
    [TestFixture]
    public class AggregateBuilderTests
    {
        [Test]
        public void EmptyBuilderReturnsExpectedProperties()
        {
            var sut = new AggregateBuilder();

            Assert.That(sut.Identifier, Is.Null);
            Assert.That(sut.ExpectedVersion, Is.EqualTo(Int32.MinValue));
            Assert.That(sut.Root, Is.Null);
        }

        [Test]
        public void IdentifiedByReturnsExpectedResult()
        {
            var sut = new AggregateBuilder();
            var expectedVersion = sut.ExpectedVersion;
            var root = sut.Root;

            var result = sut.IdentifiedBy("identifier");

            Assert.That(result, Is.SameAs(sut));
            Assert.That(sut.Identifier, Is.EqualTo("identifier"));
            Assert.That(sut.ExpectedVersion, Is.EqualTo(expectedVersion));
            Assert.That(sut.Root, Is.SameAs(root));
        }

        [Test]
        public void ExpectVersionReturnsExpectedResult()
        {
            var sut = new AggregateBuilder();
            var identifier = sut.Identifier;
            var root = sut.Root;

            var result = sut.ExpectVersion(123);

            Assert.That(result, Is.SameAs(sut));
            Assert.That(sut.Identifier, Is.EqualTo(identifier));
            Assert.That(sut.ExpectedVersion, Is.EqualTo(123));
            Assert.That(sut.Root, Is.SameAs(root));
        }

        [Test]
        public void WithRootReturnsExpectedResult()
        {
            var sut = new AggregateBuilder();
            var identifier = sut.Identifier;
            var expectedVersion = sut.ExpectedVersion;
            var root = new AggregateRootEntityStub();

            var result = sut.WithRoot(root);

            Assert.That(result, Is.SameAs(sut));
            Assert.That(sut.Identifier, Is.EqualTo(identifier));
            Assert.That(sut.ExpectedVersion, Is.EqualTo(expectedVersion));
            Assert.That(sut.Root, Is.SameAs(root));
        }

        [Test]
        public void BuildReturnsExpectedResult()
        {
            var sut = new AggregateBuilder();
            const string identifier = "identifier";
            const int expectedVersion = 123;
            var root = new AggregateRootEntityStub();

            var result = sut.IdentifiedBy(identifier).ExpectVersion(expectedVersion).WithRoot(root).Build();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Identifier, Is.EqualTo(identifier));
            Assert.That(result.ExpectedVersion, Is.EqualTo(expectedVersion));
            Assert.That(result.Root, Is.SameAs(root));
        }
    }
}
