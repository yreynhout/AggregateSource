using System;
using System.IO;
using EventStore.ClientAPI;
using NUnit.Framework;
using ProtoBuf;

namespace AggregateSource.GEventStore {
  namespace AsyncRepositoryIntegrationTests {
    [TestFixture]
    public class Construction {
      [Test]
      public void FactoryCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new AsyncRepository<AggregateRootEntityStub>(null, new ConcurrentUnitOfWork(), EmbeddedEventStore.Instance.Connection));
      }

      [Test]
      public void ConcurrentUnitOfWorkCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new AsyncRepository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, null, EmbeddedEventStore.Instance.Connection));
      }

      [Test]
      public void EventStoreConnectionCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new AsyncRepository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, new ConcurrentUnitOfWork(), null));
      }

      [Test]
      public void EventStoreReadConfigurationCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new AsyncRepository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, new ConcurrentUnitOfWork(), EmbeddedEventStore.Instance.Connection, null));
      }
    }

    [TestFixture]
    public class WithEmptyStoreAndEmptyUnitOfWork {
      AsyncRepository<AggregateRootEntityStub> _sut;
      ConcurrentUnitOfWork _unitOfWork;
      Model _model;

      [SetUp]
      public void SetUp() {
        EmbeddedEventStore.Instance.Connection.DeleteAllStreams();
        _model = new Model();
        _unitOfWork = new ConcurrentUnitOfWork();
        _sut = new AsyncRepository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, _unitOfWork, EmbeddedEventStore.Instance.Connection);
      }

      [Test]
      public void GetAsyncThrows() {
        var exception =
          Assert.Throws<AggregateException>(() => { var result = _sut.GetAsync(_model.UnknownIdentifier).Result; });
        Assert.That(exception.InnerExceptions, Has.Count.EqualTo(1));
        Assert.That(exception.InnerExceptions[0], Is.InstanceOf<AggregateNotFoundException>());
        var actualException = (AggregateNotFoundException)exception.InnerExceptions[0];
        Assert.That(actualException.Identifier, Is.EqualTo(_model.UnknownIdentifier));
        Assert.That(actualException.Type, Is.EqualTo(typeof(AggregateRootEntityStub)));
      }

      [Test]
      public void GetOptionalAsyncReturnsEmpty() {
        var result = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

        Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void AddAttachesToUnitOfWork() {
        var root = AggregateRootEntityStub.Factory();

        _sut.Add(_model.KnownIdentifier, root);

        Aggregate aggregate;
        var result = _unitOfWork.TryGet(_model.KnownIdentifier, out aggregate);
        Assert.That(result, Is.True);
        Assert.That(aggregate.Identifier, Is.EqualTo(_model.KnownIdentifier));
        Assert.That(aggregate.Root, Is.SameAs(root));
      }
    }

    [TestFixture]
    public class WithEmptyStoreAndFilledUnitOfWork {
      AsyncRepository<AggregateRootEntityStub> _sut;
      ConcurrentUnitOfWork _unitOfWork;
      AggregateRootEntityStub _root;
      Model _model;

      [SetUp]
      public void SetUp() {
        EmbeddedEventStore.Instance.Connection.DeleteAllStreams();
        _model = new Model();
        _root = AggregateRootEntityStub.Factory();
        _unitOfWork = new ConcurrentUnitOfWork();
        _unitOfWork.Attach(new Aggregate(_model.KnownIdentifier, 0, _root));
        _sut = new AsyncRepository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, _unitOfWork, EmbeddedEventStore.Instance.Connection);
      }

      [Test]
      public void GetAsyncThrowsForUnknownId() {
        var exception =
          Assert.Throws<AggregateException>(() => { var result = _sut.GetAsync(_model.UnknownIdentifier).Result; });
        Assert.That(exception.InnerExceptions, Has.Count.EqualTo(1));
        Assert.That(exception.InnerExceptions[0], Is.InstanceOf<AggregateNotFoundException>());
        var actualException = (AggregateNotFoundException)exception.InnerExceptions[0];
        Assert.That(actualException.Identifier, Is.EqualTo(_model.UnknownIdentifier));
        Assert.That(actualException.Type, Is.EqualTo(typeof(AggregateRootEntityStub)));
      }

      [Test]
      public void GetAsyncReturnsRootOfKnownId() {
        var result = _sut.GetAsync(_model.KnownIdentifier).Result;

        Assert.That(result, Is.SameAs(_root));
      }

      [Test]
      public void GetOptionalAsyncReturnsEmptyForUnknownId() {
        var result = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

        Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void GetOptionalAsyncReturnsRootForKnownId() {
        var result = _sut.GetOptionalAsync(_model.KnownIdentifier).Result;

        Assert.That(result, Is.EqualTo(new Optional<AggregateRootEntityStub>(_root)));
      }
    }

    [TestFixture]
    public class WithFilledStore {
      AsyncRepository<AggregateRootEntityStub> _sut;
      ConcurrentUnitOfWork _unitOfWork;
      AggregateRootEntityStub _root;
      Model _model;

      [SetUp]
      public void SetUp() {
        _model = new Model();
        using (var stream = new MemoryStream()) {
          Serializer.Serialize(stream, new Event());
          EmbeddedEventStore.Instance.Connection.AppendToStream(
            _model.KnownIdentifier,
            ExpectedVersion.NoStream,
            new EventData(
              Guid.NewGuid(),
              typeof(Event).AssemblyQualifiedName,
              false,
              stream.ToArray(),
              new byte[0]));
        }
        _root = AggregateRootEntityStub.Factory();
        _unitOfWork = new ConcurrentUnitOfWork();
        _sut = new AsyncRepository<AggregateRootEntityStub>(
          () => _root,
          _unitOfWork,
          EmbeddedEventStore.Instance.Connection);
      }

      [Test]
      public void GetAsyncThrowsForUnknownId() {
        var exception =
          Assert.Throws<AggregateException>(() => { var result = _sut.GetAsync(_model.UnknownIdentifier).Result; });
        Assert.That(exception.InnerExceptions, Has.Count.EqualTo(1));
        Assert.That(exception.InnerExceptions[0], Is.InstanceOf<AggregateNotFoundException>());
        var actualException = (AggregateNotFoundException)exception.InnerExceptions[0];
        Assert.That(actualException.Identifier, Is.EqualTo(_model.UnknownIdentifier));
        Assert.That(actualException.Type, Is.EqualTo(typeof(AggregateRootEntityStub)));
      }

      [Test]
      public void GetReturnsRootOfKnownId() {
        var result = _sut.GetAsync(_model.KnownIdentifier).Result;

        Assert.That(result, Is.SameAs(_root));
      }

      [Test]
      public void GetOptionalAsyncReturnsEmptyForUnknownId() {
        var result = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

        Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void GetOptionalAsyncReturnsRootForKnownId() {
        var result = _sut.GetOptionalAsync(_model.KnownIdentifier).Result;

        Assert.That(result, Is.EqualTo(new Optional<AggregateRootEntityStub>(_root)));
      }

      [ProtoContract]
      class Event { }
    }
  }
}
