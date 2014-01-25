using System;
using NUnit.Framework;

namespace AggregateSource
{
    namespace UnitOfWorkTests
    {
        public static class Model
        {
            public static readonly string KnownIdentifier = "known/identifier";
            public static readonly string UnknownIdentifier = "unknown/identifier";
        }

        [TestFixture]
        public class WithAnyInstance
        {
            UnitOfWork _sut;

            [SetUp]
            public void Setup()
            {
                _sut = new UnitOfWork();
            }

            [Test]
            public void AttachNullThrows()
            {
                Assert.Throws<ArgumentNullException>(() => _sut.Attach(null));
            }

            [Test]
            public void TryGetIdentifierCannotBeNull()
            {
                Aggregate aggregate;
                Assert.Throws<ArgumentNullException>(() => _sut.TryGet(null, out aggregate));
            }
        }

        [TestFixture]
        public class WithPristineInstance
        {
            UnitOfWork _sut;

            [SetUp]
            public void Setup()
            {
                _sut = new UnitOfWork();
            }

            [Test]
            public void AttachAggregateDoesNotThrow()
            {
                var aggregate = AggregateStubs.Stub1;
                Assert.DoesNotThrow(() => _sut.Attach(aggregate));
            }

            [Test]
            public void TryGetReturnsFalseAndNullAsAggregate()
            {
                Aggregate aggregate;
                var result = _sut.TryGet(Model.UnknownIdentifier, out aggregate);

                Assert.That(result, Is.False);
                Assert.That(aggregate, Is.Null);
            }

            [Test]
            public void HasChangesReturnsFalse()
            {
                Assert.That(_sut.HasChanges(), Is.False);
            }

            [Test]
            public void GetChangesReturnsEmpty()
            {
                Assert.That(_sut.GetChanges(), Is.EquivalentTo(new Aggregate[0]));
            }
        }

        [TestFixture]
        public class WithInstanceWithAttachedAggregate
        {
            UnitOfWork _sut;
            Aggregate _aggregate;

            [SetUp]
            public void Setup()
            {
                _aggregate = AggregateStubs.Stub1;
                _sut = new UnitOfWork();
                _sut.Attach(_aggregate);
            }

            [Test]
            public void AttachThrowsWithSameAggregate()
            {
                Assert.Throws<ArgumentException>(() => _sut.Attach(_aggregate));
            }

            [Test]
            public void AttachDoesNotThrowWithOtherAggregate()
            {
                var otherAggregate = AggregateStubs.Stub2;
                Assert.DoesNotThrow(() => _sut.Attach(otherAggregate));
            }

            [Test]
            public void TryGetReturnsFalseAndNullAsAggregateForUnknownId()
            {
                Aggregate aggregate;
                var result = _sut.TryGet(Model.UnknownIdentifier, out aggregate);

                Assert.That(result, Is.False);
                Assert.That(aggregate, Is.Null);
            }

            [Test]
            public void TryGetReturnsTrueAndAggregateForKnownId()
            {
                Aggregate aggregate;
                var result = _sut.TryGet(_aggregate.Identifier, out aggregate);

                Assert.That(result, Is.True);
                Assert.That(aggregate, Is.SameAs(_aggregate));
            }

            [Test]
            public void HasChangesReturnsFalse()
            {
                Assert.That(_sut.HasChanges(), Is.False);
            }

            [Test]
            public void GetChangesReturnsEmpty()
            {
                Assert.That(_sut.GetChanges(), Is.EquivalentTo(new Aggregate[0]));
            }
        }

        [TestFixture]
        public class WithInstanceWithAttachedChangedAggregates
        {
            UnitOfWork _sut;
            Aggregate _aggregate1;
            Aggregate _aggregate2;

            [SetUp]
            public void Setup()
            {
                _aggregate1 = AggregateStubs.Create(new ChangedAggregateRootEntityStub());
                _aggregate2 = AggregateStubs.Create(new ChangedAggregateRootEntityStub());
                _sut = new UnitOfWork();
                _sut.Attach(_aggregate1);
                _sut.Attach(_aggregate2);
            }

            [Test]
            public void HasChangesReturnsTrue()
            {
                Assert.That(_sut.HasChanges(), Is.True);
            }

            [Test]
            public void GetChangesReturnsEmpty()
            {
                Assert.That(_sut.GetChanges(), Is.EquivalentTo(new[] {_aggregate1, _aggregate2}));
            }
        }

        class ChangedAggregateRootEntityStub : AggregateRootEntity
        {
            public ChangedAggregateRootEntityStub()
            {
                ApplyChange(new object());
            }
        }
    }
}