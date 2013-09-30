using System;
using EventStore.ClientAPI.SystemData;
using NUnit.Framework;

namespace AggregateSource.EventStore.Resolvers
{
    [TestFixture]
    public class FixedStreamUserCredentialsResolverTests
    {
        FixedStreamUserCredentialsResolver _sut;
        UserCredentials _credentials;

        [SetUp]
        public void SetUp()
        {
            _credentials = new UserCredentials("admin", "changeit");
            _sut = new FixedStreamUserCredentialsResolver(_credentials);
        }

        [Test]
        public void FixedUserCredentialsCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new FixedStreamUserCredentialsResolver(null));
        }

        [Test]
        public void IsStreamUserCredentialsResolver()
        {
            Assert.That(_sut, Is.InstanceOf<IStreamUserCredentialsResolver>());
        }

        [Test]
        public void ResolveIdentifierCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Resolve(null));
        }

        [Test]
        public void ResolveReturnsExpectedResult()
        {
            var identifier = Guid.NewGuid().ToString();

            var result = _sut.Resolve(identifier);

            Assert.That(result, Is.SameAs(_credentials));
        }
    }
}