using System;
using AggregateSource.EventStore.Builders;
using AggregateSource.EventStore.Stubs;
using NUnit.Framework;

namespace AggregateSource.EventStore.Snapshots
{
    [TestFixture]
    public class SnapshotReaderConfigurationTests
    {
        SnapshotReaderConfigurationBuilder _sutBuilder;

        [SetUp]
        public void SetUp()
        {
            _sutBuilder = SnapshotReaderConfigurationBuilder.Default;
        }

        [Test]
        public void DeserializerCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sutBuilder.UsingDeserializer(null).Build());
        }

        [Test]
        public void StreamNameResolverCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sutBuilder.UsingStreamNameResolver(null).Build());
        }

        [Test]
        public void StreamUserCredentialsResolverCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sutBuilder.UsingStreamUserCredentialsResolver(null).Build());
        }

        [Test]
        public void UsingConstructorReturnsInstanceWithExpectedProperties()
        {
            var sut = _sutBuilder.Build();

            Assert.That(sut.Deserializer, Is.SameAs(StubbedSnapshotDeserializer.Instance));
            Assert.That(sut.StreamNameResolver, Is.SameAs(StubbedStreamNameResolver.Instance));
            Assert.That(sut.StreamUserCredentialsResolver, Is.SameAs(StubbedStreamUserCredentialsResolver.Instance));
        }
    }
}