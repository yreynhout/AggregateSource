using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource
{
    [TestFixture]
    public class AggregateTests
    {
        [Test, Combinatorial]
        public void UsingDefaultConstructorReturnsInstanceWithExpectedProperties(
            [ValueSource(typeof (AggregateTestsValueSource), "IdSource")] string identifier,
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

        [Test]
        public void DoesEqualItSelf()
        {
            var sut = SutFactory();
            var self = sut;
            Assert.That(sut.Equals(self), Is.True);
        }

        [Test]
        public void DoesNotEqualNull()
        {
            var sut = SutFactory();
            Assert.That(sut.Equals(null), Is.False);
        }

        [Test]
        public void DoesNotEqualObjectOfAnotherType()
        {
            var sut = SutFactory();
            Assert.That(sut.Equals(new object()), Is.False);
        }

        [Test]
        public void TwoInstancesAreEqualIfTheirValuesAreEqual()
        {
            var root = new AggregateRootEntityStub();
            var instance1 = SutFactory("123", 123, root);
            var instance2 = SutFactory("123", 123, root);
            Assert.That(instance1.Equals(instance2), Is.True);
        }

        [Test]
        public void TwoInstancesAreNotEqualIfTheirIdentifierDiffers()
        {
            var root = new AggregateRootEntityStub();
            var instance1 = SutFactory("123", 123, root);
            var instance2 = SutFactory("456", 123, root);
            Assert.That(instance1.Equals(instance2), Is.False);
        }

        [Test]
        public void TwoInstancesAreNotEqualIfTheirExpectedVersionDiffers()
        {
            var root = new AggregateRootEntityStub();
            var instance1 = SutFactory("123", 123, root);
            var instance2 = SutFactory("123", 456, root);
            Assert.That(instance1.Equals(instance2), Is.False);
        }

        [Test]
        public void TwoInstancesAreNotEqualIfTheirRootDiffers()
        {
            var root1 = new AggregateRootEntityStub();
            var root2 = new AggregateRootEntityStub();
            var instance1 = SutFactory("123", 123, root1);
            var instance2 = SutFactory("123", 123, root2);
            Assert.That(instance1.Equals(instance2), Is.False);
        }

        [Test]
        public void TwoInstancesHaveTheSameHashCodeIfTheirValuesAreEqual()
        {
            var root = new AggregateRootEntityStub();
            var instance1 = SutFactory("123", 123, root);
            var instance2 = SutFactory("123", 123, root);
            Assert.That(instance1.GetHashCode().Equals(instance2.GetHashCode()), Is.True);
        }

        [Test]
        public void TwoInstancesDoNotHaveTheSameHashCodeIfTheirIdentifierDiffers()
        {
            var root = new AggregateRootEntityStub();
            var instance1 = SutFactory("123", 123, root);
            var instance2 = SutFactory("456", 123, root);
            Assert.That(instance1.GetHashCode().Equals(instance2.GetHashCode()), Is.False);
        }

        [Test]
        public void TwoInstancesDoNotHaveTheSameHashCodeIfTheirExpectedVersionDiffers()
        {
            var root = new AggregateRootEntityStub();
            var instance1 = SutFactory("123", 123, root);
            var instance2 = SutFactory("123", 456, root);
            Assert.That(instance1.GetHashCode().Equals(instance2.GetHashCode()), Is.False);
        }

        [Test]
        public void TwoInstancesDoNotHaveTheSameHashCodeIfTheirRootDiffers()
        {
            var root1 = new AggregateRootEntityStub();
            var root2 = new AggregateRootEntityStub();
            var instance1 = SutFactory("123", 123, root1);
            var instance2 = SutFactory("123", 123, root2);
            Assert.That(instance1.GetHashCode().Equals(instance2.GetHashCode()), Is.False);
        }

        private static Aggregate SutFactory()
        {
            const string identifier = "identifier";
            const int expectedVersion = 123;
            var root = new AggregateRootEntityStub();
            return SutFactory(identifier, expectedVersion, root);
        }

        private static Aggregate SutFactory(string identifier, int expectedVersion, IAggregateRootEntity root)
        {
            return new Aggregate(identifier, expectedVersion, root);
        }
    }
}