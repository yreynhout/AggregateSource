using System;
using System.Collections.Generic;
using System.Linq;
using AggregateSource;
using NUnit.Framework;

namespace StreamSource {
  namespace RepositoryTests {
    [TestFixture]
    public class Construction {
      [Test]
      public void FactoryCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new Repository<AggregateRootEntityStub>(null, new UnitOfWork(), EventStreamReaderStub.Instance));
      }

      [Test]
      public void UnitOfWorkCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new Repository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, null, EventStreamReaderStub.Instance));
      }

      [Test]
      public void EventStreamReaderCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new Repository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, new UnitOfWork(), null));
      }
    }
    
    class EventStreamReaderStub : IEventStreamReader {
      public static readonly IEventStreamReader Instance = new EventStreamReaderStub();
      public Optional<EventStream> Read(Guid id) {
        return Optional<EventStream>.Empty;
      }
    }

    [TestFixture]
    public class WithEmptyStoreAndEmptyUnitOfWork {
      Repository<AggregateRootEntityStub> _sut;
      UnitOfWork _unitOfWork;

      [SetUp]
      public void SetUp() {
        _unitOfWork = new UnitOfWork();
        _sut = new Repository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, _unitOfWork, new EmptyEventStreamReader());
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
      public void GetOptionalReturnsEmpty() {
        var result = _sut.GetOptional(Guid.NewGuid());

        Assert.That(result, Is.SameAs(Optional<AggregateRootEntityStub>.Empty));
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
      AggregateRootEntityStub _root;
      Guid _id;

      [SetUp]
      public void SetUp() {
        _id = Guid.NewGuid();
        _root = AggregateRootEntityStub.Factory();
        _unitOfWork = new UnitOfWork();
        _unitOfWork.Attach(new Aggregate(_id, 0, _root));
        _sut = new Repository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, _unitOfWork, new EmptyEventStreamReader());
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
      public void GetOptionalReturnsEmptyForUnknownId() {
        var result = _sut.GetOptional(Guid.NewGuid());

        Assert.That(result, Is.SameAs(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void GetOptionalReturnsRootForKnownId() {
        var result = _sut.GetOptional(_id);

        Assert.That(result, Is.EqualTo(new Optional<AggregateRootEntityStub>(_root)));
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
          new FilledEventStreamReader(new Dictionary<Guid, IList<object>> {
            {_id, new List<object>()}
          }));
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
      public void GetOptionalReturnsEmptyForUnknownId() {
        var result = _sut.GetOptional(Guid.NewGuid());

        Assert.That(result, Is.SameAs(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void GetOptionalReturnsRootForKnownId() {
        var result = _sut.GetOptional(_id);

        Assert.That(result, Is.EqualTo(new Optional<AggregateRootEntityStub>(_root)));
      }
    }

    class EmptyEventStreamReader : IEventStreamReader {
      public Optional<EventStream> Read(Guid id) {
        return Optional<EventStream>.Empty;
      }
    }
    
    class FilledEventStreamReader : IEventStreamReader {
      readonly Dictionary<Guid, IList<object>> _storage;

      public FilledEventStreamReader(Dictionary<Guid, IList<object>> storage) {
        if (storage == null) throw new ArgumentNullException("storage");
        _storage = storage;
      }

      public Optional<EventStream> Read(Guid id) {
        IList<object> events;
        return _storage.TryGetValue(id, out events) ? 
          new Optional<EventStream>(new EventStream(events.Count, events.ToArray())) : 
          Optional<EventStream>.Empty;
      }
    }

    //TODO: Add tests that prove casting throws when types mismatch
  }
}
