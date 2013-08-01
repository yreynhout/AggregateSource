using System;
using AggregateSource.GEventStore.Framework;
using NUnit.Framework;

namespace AggregateSource.GEventStore
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

            [Test]
            public void ReadVersionCanNotBeNegative()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { var _ = _sut.Read(_model.KnownIdentifier, -1); });
            }

            [Test]
            public void ReadVersionCanNotBeZero()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { var _ = _sut.Read(_model.KnownIdentifier, 0); });
            }

            [Test]
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
