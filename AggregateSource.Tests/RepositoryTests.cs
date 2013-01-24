using System;
using System.Linq;
using NUnit.Framework;

namespace AggregateSource.Tests {
  namespace RepositoryTests {
    [TestFixture]
    public class Construction {
      [Test]
      public void DefaultCtorDoesNotThrow() {
        Assert.DoesNotThrow(() => new NullRepository());
      }

      [Test]
      public void UnitOfWorkCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new NullRepository(null));
      }
    }

    [TestFixture]
    public class WithEmptyStoreAndEmptyUnitOfWork {
      Repository<DummyAggregateRootEntity> _sut;
      UnitOfWork _unitOfWork;

      [SetUp]
      public void SetUp() {
        _unitOfWork = new UnitOfWork();
        _sut = new EmptyStoreRepository<DummyAggregateRootEntity>(_unitOfWork);
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
        Assert.That(aggregate.Root, Is.SameAs(root));
      }
    }

    [TestFixture]
    public class WithEmptyStoreAndEmptyAmbientUnitOfWork {
      Repository<DummyAggregateRootEntity> _sut;
      UnitOfWork _unitOfWork;
      UnitOfWorkScope _scope;

      [SetUp]
      public void SetUp() {
        _unitOfWork = new UnitOfWork();
        _scope = new UnitOfWorkScope(_unitOfWork);
        _sut = new EmptyStoreRepository<DummyAggregateRootEntity>(_unitOfWork);
      }

      [TearDown]
      public void TearDown() {
        if(_scope != null) _scope.Dispose();
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
        Assert.That(aggregate.Root, Is.SameAs(root));
      }
    }

    [TestFixture]
    public class WithEmptyStoreAndFilledUnitOfWork {
      Repository<DummyAggregateRootEntity> _sut;
      UnitOfWork _unitOfWork;
      Aggregate _aggregate;

      [SetUp]
      public void SetUp() {
        _aggregate = new Aggregate(Guid.NewGuid(), new DummyAggregateRootEntity());
        var unitOfWork = new UnitOfWork();
        foreach (var aggregate in new[] { _aggregate }) {
          unitOfWork.Attach(aggregate);
        }
        _unitOfWork = unitOfWork;
        _sut = new EmptyStoreRepository<DummyAggregateRootEntity>(_unitOfWork);
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
    public class WithEmptyStoreAndFilledAmbientUnitOfWork {
      Repository<DummyAggregateRootEntity> _sut;
      UnitOfWork _unitOfWork;
      Aggregate _aggregate;
      UnitOfWorkScope _scope;

      [SetUp]
      public void SetUp() {
        _unitOfWork = new UnitOfWork();
        _scope = new UnitOfWorkScope(_unitOfWork);
        _aggregate = new Aggregate(Guid.NewGuid(), new DummyAggregateRootEntity());
        var unitOfWork = new UnitOfWork();
        foreach (var aggregate in new[] { _aggregate }) {
          unitOfWork.Attach(aggregate);
        }
        _unitOfWork = unitOfWork;
        _sut = new EmptyStoreRepository<DummyAggregateRootEntity>(_unitOfWork);
      }

      [TearDown]
      public void TearDown() {
        if (_scope != null) _scope.Dispose();
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
    public class WithFilledStore {
      Repository<DummyAggregateRootEntity> _sut;
      UnitOfWork _unitOfWork;
      Aggregate _aggregate;

      [SetUp]
      public void SetUp() {
        _aggregate = new Aggregate(Guid.NewGuid(), new DummyAggregateRootEntity());
        _unitOfWork = new UnitOfWork();
        _sut = new FilledStoreRepository<DummyAggregateRootEntity>(_unitOfWork, new[] { _aggregate });
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

    class NullRepository : Repository<DummyAggregateRootEntity> {
      public NullRepository() { }
      public NullRepository(UnitOfWork unitOfWork) : base(unitOfWork) { }
      protected override bool TryReadAggregate(Guid id, out Aggregate aggregate) {
        throw new NotSupportedException();
      }

      protected override Aggregate CreateAggregate(Guid id, DummyAggregateRootEntity root) {
        throw new NotSupportedException();
      }
    }

    class EmptyStoreRepository<TAggregateRoot> : Repository<TAggregateRoot> where TAggregateRoot : AggregateRootEntity {
      public EmptyStoreRepository() { }
      public EmptyStoreRepository(UnitOfWork unitOfWork) : base(unitOfWork) { }

      protected override bool TryReadAggregate(Guid id, out Aggregate aggregate) {
        aggregate = null;
        return false;
      }

      protected override Aggregate CreateAggregate(Guid id, TAggregateRoot root) {
        return new Aggregate(id, root);
      }
    }

    class FilledStoreRepository<TAggregateRoot> : Repository<TAggregateRoot> where TAggregateRoot : AggregateRootEntity {
      readonly Aggregate[] _storage;

      public FilledStoreRepository(Aggregate[] storage) {
        if (storage == null) throw new ArgumentNullException("storage");
        _storage = storage;
      }

      public FilledStoreRepository(UnitOfWork unitOfWork, Aggregate[] storage)
        : base(unitOfWork) {
        if (storage == null) throw new ArgumentNullException("storage");
        _storage = storage;
      }

      protected override bool TryReadAggregate(Guid id, out Aggregate aggregate) {
        aggregate = _storage.SingleOrDefault(candidate => candidate.Id == id);
        return aggregate != null;
      }

      protected override Aggregate CreateAggregate(Guid id, TAggregateRoot root) {
        return new Aggregate(id, root);
      }
    }

    //TODO: Add tests that prove casting throws when types mismatch

    class DummyAggregateRootEntity : AggregateRootEntity { }
  }
}
