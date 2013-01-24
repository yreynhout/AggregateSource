using System;
using System.Linq;
using NUnit.Framework;

namespace AggregateSource.Tests {
  namespace UnitOfWorkTests {
    [TestFixture]
    public class WithPristineInstance {
      UnitOfWork _sut;

      [SetUp]
      public void SetUp() {
        _sut = new UnitOfWork();
      }

      [Test]
      public void AttachNullThrows() {
        Assert.Throws<ArgumentNullException>(() => _sut.Attach(null));
      }

      [Test]
      public void AttachAggregateDoesNotThrow() {
        var aggregate = new Aggregate(Guid.NewGuid(), new DummyAggregateRootEntity());
        Assert.DoesNotThrow(() => _sut.Attach(aggregate));
      }

      [Test]
      public void TryGetReturnsFalseAndNullAsAggregate() {
        Aggregate aggregate;
        var result = _sut.TryGet(Guid.NewGuid(), out aggregate);

        Assert.That(result, Is.False);
        Assert.That(aggregate, Is.Null);
      }

      [Test]
      public void HasChangesReturnsFalse() {
        Assert.That(_sut.HasChanges(), Is.False);
      }

      [Test]
      public void GetChangesReturnsEmpty() {
        Assert.That(_sut.GetChanges(), Is.EquivalentTo(Enumerable.Empty<Aggregate>()));
      }
    }

    [TestFixture]
    public class WithInstanceWithAttachedAggregate {
      UnitOfWork _sut;
      Aggregate _aggregate;

      [SetUp]
      public void SetUp() {
        _aggregate = new Aggregate(Guid.NewGuid(), new DummyAggregateRootEntity());
        _sut = new UnitOfWork();
        _sut.Attach(_aggregate);
      }

      [Test]
      public void AttachThrowsWithSameAggregate() {
        Assert.Throws<ArgumentException>(() => _sut.Attach(_aggregate));
      }

      [Test]
      public void AttachDoesNotThrowWithOtherAggregate() {
        var otherAggregate = new Aggregate(Guid.NewGuid(), new DummyAggregateRootEntity());
        Assert.DoesNotThrow(() => _sut.Attach(otherAggregate));
      }

      [Test]
      public void TryGetReturnsFalseAndNullAsAggregateForUnknownId() {
        Aggregate aggregate;
        var result = _sut.TryGet(Guid.NewGuid(), out aggregate);

        Assert.That(result, Is.False);
        Assert.That(aggregate, Is.Null);
      }

      [Test]
      public void TryGetReturnsTrueAndAggregateForKnownId() {
        Aggregate aggregate;
        var result = _sut.TryGet(_aggregate.Id, out aggregate);

        Assert.That(result, Is.True);
        Assert.That(aggregate, Is.SameAs(_aggregate));
      }

      [Test]
      public void HasChangesReturnsFalse() {
        Assert.That(_sut.HasChanges(), Is.False);
      }

      [Test]
      public void GetChangesReturnsEmpty() {
        Assert.That(_sut.GetChanges(), Is.EquivalentTo(Enumerable.Empty<Aggregate>()));
      }
    }

    [TestFixture]
    public class WithInstanceWithAttachedChangedAggregates {
      UnitOfWork _sut;
      Aggregate _aggregate1;
      Aggregate _aggregate2;

      [SetUp]
      public void SetUp() {
        _aggregate1 = new Aggregate(Guid.NewGuid(), new ChangedAggregateRootEntity());
        _aggregate2 = new Aggregate(Guid.NewGuid(), new ChangedAggregateRootEntity());
        _sut = new UnitOfWork();
        _sut.Attach(_aggregate1);
        _sut.Attach(_aggregate2);
      }

      [Test]
      public void HasChangesReturnsTrue() {
        Assert.That(_sut.HasChanges(), Is.True);
      }

      [Test]
      public void GetChangesReturnsEmpty() {
        Assert.That(_sut.GetChanges(), Is.EquivalentTo(new [] { _aggregate1, _aggregate2 }));
      }
    }

    class DummyAggregateRootEntity : AggregateRootEntity { }
    class ChangedAggregateRootEntity : AggregateRootEntity {
      public ChangedAggregateRootEntity() {
        Apply(new object());
      }
    }
  }
}
