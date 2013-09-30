using System;
using AggregateSource.EventStore.Framework;
using NUnit.Framework;

namespace AggregateSource.EventStore
{
    namespace EventReaderTests
    {
        [TestFixture]
        public class WithAnyInstance
        {
            Model _model;
            EventReader _sut;

            [SetUp]
            public void SetUp()
            {
                _model = new Model();
                _sut = EventReaderFactory.Create();
            }

            [Test]
            public void ReadIdentifierCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => { var _ = _sut.Read(null, 0); });
            }

            [Test, Ignore("Need to figure out if we're gonna throw if the version is <= 0")]
            public void ReadVersionCanNotBeNegative()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { var _ = _sut.Read(_model.KnownIdentifier, -1); });
            }

            [Test, Ignore("Need to figure out if we're gonna throw if the version is <= 0")]
            public void ReadVersionCanNotBeZero()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { var _ = _sut.Read(_model.KnownIdentifier, 0); });
            }

            [Test, Ignore("Need to figure out if we're gonna throw if the version is <= 0")]
            public void ReadVersionCanBePositive()
            {
                Assert.That(_sut.Read(_model.KnownIdentifier, 1), Is.Empty);
            }
        }

        [TestFixture]
        public class WithEmptyStore{}

        [TestFixture]
        public class WithStreamInStore {}

        [TestFixture]
        public class WithDeletedStreamInStore{}
    }
}
