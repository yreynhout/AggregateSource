using System;
using System.Linq;
using NUnit.Framework;

namespace AggregateSource.Tests {
  namespace RepositoryTests {
    [TestFixture]
    public class Construction {
      [Test]
      public void ReaderCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new Repository<DummyAggregateRootEntity>(null, new UnitOfWork()));
      }

      [Test]
      public void UnitOfWorkCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new Repository<DummyAggregateRootEntity>(id => null, null));
      }
    }

    [TestFixture]
    public class WithAggregateNotFoundReaderAndEmptyUnitOfWork {
      Repository<DummyAggregateRootEntity> _sut;
      UnitOfWork _unitOfWork;

      [SetUp]
      public void SetUp() {
        _unitOfWork = Factory.EmptyUnitOfWork();
        _sut = new Repository<DummyAggregateRootEntity>(
          Factory.AggregateNotFoundReader(),
          _unitOfWork);
      }

      [Test]
      public void GetThrows() {
        var id = Guid.NewGuid();
        var exception =
          Assert.Throws<AggregateNotFoundException>(() => _sut.Get(id));
        Assert.That(exception.AggregateId, Is.EqualTo(id));
        Assert.That(exception.AggregateType, Is.EqualTo(typeof(DummyAggregateRootEntity)));
      }

      [Test]
      public void TryGetReturnsFalseAndNull() {
        DummyAggregateRootEntity root;
        var result = _sut.TryGet(Guid.NewGuid(), out root);

        Assert.That(result, Is.False);
        Assert.That(root, Is.Null);
      }

      [Test]
      public void AddAttachesToUnitOfWork() {
        var id = Guid.NewGuid();
        var root = new DummyAggregateRootEntity();

        _sut.Add(id, root);

        Aggregate aggregate;
        var result = _unitOfWork.TryGet(id, out aggregate);
        Assert.That(result, Is.True);
        Assert.That(aggregate.Id, Is.EqualTo(id));
        Assert.That(aggregate.Version, Is.EqualTo(Aggregate.InitialVersion));
        Assert.That(aggregate.Root, Is.SameAs(root));
      }
    }

    [TestFixture]
    public class WithAggregateNotFoundReaderAndFilledUnitOfWork {
      Repository<DummyAggregateRootEntity> _sut;
      UnitOfWork _unitOfWork;
      Aggregate _aggregate;

      [SetUp]
      public void SetUp() {
        _aggregate = new Aggregate(Guid.NewGuid(), Aggregate.InitialVersion, new DummyAggregateRootEntity());
        _unitOfWork = Factory.FilledUnitOfWork(new[] { _aggregate });
        _sut = new Repository<DummyAggregateRootEntity>(
          Factory.AggregateNotFoundReader(),
          _unitOfWork);
      }

      [Test]
      public void GetThrowsForUnknownId() {
        var id = Guid.NewGuid();
        var exception =
          Assert.Throws<AggregateNotFoundException>(() => _sut.Get(id));
        Assert.That(exception.AggregateId, Is.EqualTo(id));
        Assert.That(exception.AggregateType, Is.EqualTo(typeof(DummyAggregateRootEntity)));
      }

      [Test]
      public void GetReturnsRootOfKnownId() {
        var result = _sut.Get(_aggregate.Id);

        Assert.That(result, Is.SameAs(_aggregate.Root));
      }

      [Test]
      public void TryGetReturnsFalseAndNullForUnknownId() {
        DummyAggregateRootEntity root;
        var result = _sut.TryGet(Guid.NewGuid(), out root);

        Assert.That(result, Is.False);
        Assert.That(root, Is.Null);
      }

      [Test]
      public void TryGetReturnsTrueAndRootForKnownId() {
        DummyAggregateRootEntity root;
        var result = _sut.TryGet(_aggregate.Id, out root);

        Assert.That(result, Is.True);
        Assert.That(root, Is.SameAs(_aggregate.Root));
      }
    }

    [TestFixture]
    public class WithAggregateReaderAndEmptyUnitOfWork {
      Repository<DummyAggregateRootEntity> _sut;
      UnitOfWork _unitOfWork;
      Aggregate _aggregate;

      [SetUp]
      public void SetUp() {
        _aggregate = new Aggregate(Guid.NewGuid(), Aggregate.InitialVersion, new DummyAggregateRootEntity());
        _unitOfWork = Factory.EmptyUnitOfWork();
        _sut = new Repository<DummyAggregateRootEntity>(
          Factory.AggregateReader(new[] { _aggregate }),
          _unitOfWork);
      }

      [Test]
      public void GetThrowsForUnknownId() {
        var id = Guid.NewGuid();
        var exception =
          Assert.Throws<AggregateNotFoundException>(() => _sut.Get(id));
        Assert.That(exception.AggregateId, Is.EqualTo(id));
        Assert.That(exception.AggregateType, Is.EqualTo(typeof(DummyAggregateRootEntity)));
      }

      [Test]
      public void GetReturnsRootOfKnownId() {
        var result = _sut.Get(_aggregate.Id);

        Assert.That(result, Is.SameAs(_aggregate.Root));
      }

      [Test]
      public void TryGetReturnsFalseAndNullForUnknownId() {
        DummyAggregateRootEntity root;
        var result = _sut.TryGet(Guid.NewGuid(), out root);

        Assert.That(result, Is.False);
        Assert.That(root, Is.Null);
      }

      [Test]
      public void TryGetReturnsTrueAndRootForKnownId() {
        DummyAggregateRootEntity root;
        var result = _sut.TryGet(_aggregate.Id, out root);

        Assert.That(result, Is.True);
        Assert.That(root, Is.SameAs(_aggregate.Root));
      }
    }

    //TODO: Add tests that prove casting throws when types mismatch

    static class Factory {
      public static Func<Guid, Aggregate> AggregateNotFoundReader() {
        return id => null;
      }

      public static Func<Guid, Aggregate> AggregateReader(Aggregate[] storage) {
        return id => storage.SingleOrDefault(aggregate => aggregate.Id == id);
      }

      public static UnitOfWork EmptyUnitOfWork() {
        return new UnitOfWork();
      }

      public static UnitOfWork FilledUnitOfWork(Aggregate[] aggregates) {
        var unitOfWork = EmptyUnitOfWork();
        foreach (var aggregate in aggregates) {
          unitOfWork.Attach(aggregate);
        }
        return unitOfWork;
      }
    }

    class DummyAggregateRootEntity : AggregateRootEntity { }
  }
}
