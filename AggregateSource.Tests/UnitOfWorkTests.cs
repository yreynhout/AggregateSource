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

    [TestFixture]
    public class InsideScope {
      UnitOfWork _sut;
      UnitOfWorkScope _scope;

      [SetUp]
      public void SetUp() {
        _sut = new UnitOfWork();
        _scope = new UnitOfWorkScope(_sut);
      }

      [TearDown]
      public void TearDown() {
        if (_scope != null) _scope.Dispose();
      }

      [Test]
      public void CurrentOrNullReturnsScope() {
        Assert.That(UnitOfWork.CurrentOrNull, Is.SameAs(_sut));
      }

      [Test]
      public void CurrentReturnsScope() {
        Assert.That(UnitOfWork.Current, Is.SameAs(_sut));
      }

      [Test]
      public void TryGetCurrentReturnsTrueAndScope() {
        UnitOfWork unitOfWork;
        var result = UnitOfWork.TryGetCurrent(out unitOfWork);

        Assert.That(result, Is.True);
        Assert.That(unitOfWork, Is.SameAs(_sut));
      }
    }

    [TestFixture]
    public class OutsideScope {
      [Test]
      public void CurrentOrNullReturnsNull() {
        Assert.IsNull(UnitOfWork.CurrentOrNull);
      }

      [Test]
      public void CurrentThrows() {
        Assert.Throws<UnitOfWorkScopeException>(() => { var current = UnitOfWork.Current; });
      }

      [Test]
      public void TryGetCurrentReturnsFalseAndNull() {
        UnitOfWork unitOfWork;
        var result = UnitOfWork.TryGetCurrent(out unitOfWork);

        Assert.That(result, Is.False);
        Assert.That(unitOfWork, Is.Null);
      }

    }
  }
}
