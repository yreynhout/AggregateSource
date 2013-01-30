using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AggregateSource.Tests {
  namespace RepositoryTests {
    [TestFixture]
    public class Construction {
      [Test]
      public void FactoryCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new Repository<AggregateRootEntityStub>(null, new UnitOfWork(), id => null));
      }

      [Test]
      public void UnitOfWorkCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new Repository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, null, id => null));
      }

      [Test]
      public void EventStreamReaderCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new Repository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, new UnitOfWork(), null));
      }
    }

    [TestFixture]
    public class WithEmptyStoreAndEmptyUnitOfWork {
      Repository<AggregateRootEntityStub> _sut;
      UnitOfWork _unitOfWork;

      [SetUp]
      public void SetUp() {
        _unitOfWork = new UnitOfWork();
        _sut = new Repository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, _unitOfWork, id => null);
      }

      [Test]
      public void GetThrows() {
        var id = Guid.NewGuid();
        var exception =
          Assert.Throws<AggregateNotFoundException>(() => _sut.Get(id));
        Assert.That(exception.AggregateId, Is.EqualTo(id));
        Assert.That(exception.AggregateType, Is.EqualTo(typeof(AggregateRootEntityStub)));
      }

      [Test]
      public void TryGetReturnsFalseAndNull() {
        AggregateRootEntityStub root;
        var result = _sut.TryGet(Guid.NewGuid(), out root);

        Assert.That(result, Is.False);
        Assert.That(root, Is.Null);
      }

      [Test]
      public void AddAttachesToUnitOfWork() {
        var id = Guid.NewGuid();
        var root = AggregateRootEntityStub.Factory();

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
      Repository<AggregateRootEntityStub> _sut;
      UnitOfWork _unitOfWork;
      Aggregate _aggregate;

      [SetUp]
      public void SetUp() {
        _aggregate = AggregateStubs.Stub1;
        _unitOfWork = new UnitOfWork();
        foreach (var aggregate in new[] { _aggregate }) {
          _unitOfWork.Attach(aggregate);
        }
        _sut = new Repository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, _unitOfWork, id => null);
      }

      [Test]
      public void GetThrowsForUnknownId() {
        var id = Guid.NewGuid();
        var exception =
          Assert.Throws<AggregateNotFoundException>(() => _sut.Get(id));
        Assert.That(exception.AggregateId, Is.EqualTo(id));
        Assert.That(exception.AggregateType, Is.EqualTo(typeof(AggregateRootEntityStub)));
      }

      [Test]
      public void GetReturnsRootOfKnownId() {
        var result = _sut.Get(_aggregate.Id);

        Assert.That(result, Is.SameAs(_aggregate.Root));
      }

      [Test]
      public void TryGetReturnsFalseAndNullForUnknownId() {
        AggregateRootEntityStub root;
        var result = _sut.TryGet(Guid.NewGuid(), out root);

        Assert.That(result, Is.False);
        Assert.That(root, Is.Null);
      }

      [Test]
      public void TryGetReturnsTrueAndRootForKnownId() {
        AggregateRootEntityStub root;
        var result = _sut.TryGet(_aggregate.Id, out root);

        Assert.That(result, Is.True);
        Assert.That(root, Is.SameAs(_aggregate.Root));
      }
    }

    [TestFixture]
    public class WithFilledStore {
      Repository<AggregateRootEntityStub> _sut;
      UnitOfWork _unitOfWork;
      AggregateRootEntityStub _root;
      Guid _id;

      [SetUp]
      public void SetUp() {
        _id = Guid.NewGuid();
        _root = AggregateRootEntityStub.Factory();
        _unitOfWork = new UnitOfWork();
        _sut = new Repository<AggregateRootEntityStub>(
          () => _root, 
          _unitOfWork, 
          id => id == _id ? new Tuple<int, IEnumerable<object>>(0, new object[0]) : null);
      }

      [Test]
      public void GetThrowsForUnknownId() {
        var id = Guid.NewGuid();
        var exception =
          Assert.Throws<AggregateNotFoundException>(() => _sut.Get(id));
        Assert.That(exception.AggregateId, Is.EqualTo(id));
        Assert.That(exception.AggregateType, Is.EqualTo(typeof(AggregateRootEntityStub)));
      }

      [Test]
      public void GetReturnsRootOfKnownId() {
        var result = _sut.Get(_id);

        Assert.That(result, Is.SameAs(_root));
      }

      [Test]
      public void TryGetReturnsFalseAndNullForUnknownId() {
        AggregateRootEntityStub root;
        var result = _sut.TryGet(Guid.NewGuid(), out root);

        Assert.That(result, Is.False);
        Assert.That(root, Is.Null);
      }

      [Test]
      public void TryGetReturnsTrueAndRootForKnownId() {
        AggregateRootEntityStub root;
        var result = _sut.TryGet(_id, out root);

        Assert.That(result, Is.True);
        Assert.That(root, Is.SameAs(_root));
      }
    }

    //TODO: Add tests that prove casting throws when types mismatch
  }
}
