using System;
using System.IO;
using AggregateSource.GEventStore.Framework;
using EventStore.ClientAPI;
using FakeItEasy;
using NUnit.Framework;

namespace AggregateSource.GEventStore {
  namespace AsyncRepositoryTests {
    [TestFixture]
    public class Construction {
      [Test]
      public void FactoryCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new AsyncRepository<AggregateRootEntityStub>(null, new ConcurrentUnitOfWork(), EmbeddedEventStore.Instance.Connection, EventReaderConfigurationFactory.Create()));
      }

      [Test]
      public void ConcurrentUnitOfWorkCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new AsyncRepository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, null, EmbeddedEventStore.Instance.Connection, EventReaderConfigurationFactory.Create()));
      }

      [Test]
      public void EventStoreConnectionCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new AsyncRepository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, new ConcurrentUnitOfWork(), null, EventReaderConfigurationFactory.Create()));
      }

      [Test]
      public void EventReaderConfigurationCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new AsyncRepository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, new ConcurrentUnitOfWork(), EmbeddedEventStore.Instance.Connection, null));
      }
    }

    [TestFixture]
    public class WithEmptyStoreAndEmptyUnitOfWork {
      AsyncRepository<AggregateRootEntityStub> _sut;
      ConcurrentUnitOfWork _unitOfWork;
      Model _model;
      IStreamNameResolver _streamNameResolver;

      [SetUp]
      public void SetUp() {
        EmbeddedEventStore.Instance.Connection.DeleteAllStreams();
        _model = new Model();
        _unitOfWork = new ConcurrentUnitOfWork();
        _streamNameResolver = A.Fake<IStreamNameResolver>();
        A.CallTo(() => _streamNameResolver.Resolve(_model.KnownIdentifier)).Returns(_model.KnownIdentifier);
        A.CallTo(() => _streamNameResolver.Resolve(_model.UnknownIdentifier)).Returns(_model.UnknownIdentifier);
        _sut = new AsyncRepository<AggregateRootEntityStub>(
          AggregateRootEntityStub.Factory, 
          _unitOfWork, 
          EmbeddedEventStore.Instance.Connection, 
          EventReaderConfigurationFactory.CreateWithResolver(_streamNameResolver));
      }

      [Test]
      public void GetAsyncThrows() {
        var exception =
          Assert.Throws<AggregateException>(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier).Result; });
        Assert.That(exception.InnerExceptions, Has.Count.EqualTo(1));
        Assert.That(exception.InnerExceptions[0], Is.InstanceOf<AggregateNotFoundException>());
        var actualException = (AggregateNotFoundException)exception.InnerExceptions[0];
        Assert.That(actualException.Identifier, Is.EqualTo(_model.UnknownIdentifier));
        Assert.That(actualException.Type, Is.EqualTo(typeof(AggregateRootEntityStub)));
      }

      [Test]
      public void GetAsyncResolvesStreamName() {
        Catch.ExceptionOf(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier).Result; });

        A.CallTo(() => _streamNameResolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalAsyncReturnsEmpty() {
        var result = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

        Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void GetOptionalAsyncResolvesStreamName() {
        var _ = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

        A.CallTo(() => _streamNameResolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
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
      IStreamNameResolver _streamNameResolver;

      [SetUp]
      public void SetUp() {
        EmbeddedEventStore.Instance.Connection.DeleteAllStreams();
        _model = new Model();
        _root = AggregateRootEntityStub.Factory();
        _unitOfWork = new ConcurrentUnitOfWork();
        _unitOfWork.Attach(new Aggregate(_model.KnownIdentifier, 0, _root));
        _streamNameResolver = A.Fake<IStreamNameResolver>();
        A.CallTo(() => _streamNameResolver.Resolve(_model.KnownIdentifier)).Returns(_model.KnownIdentifier);
        A.CallTo(() => _streamNameResolver.Resolve(_model.UnknownIdentifier)).Returns(_model.UnknownIdentifier);
        _sut = new AsyncRepository<AggregateRootEntityStub>(
          AggregateRootEntityStub.Factory, 
          _unitOfWork, 
          EmbeddedEventStore.Instance.Connection, 
          EventReaderConfigurationFactory.CreateWithResolver(_streamNameResolver));
      }

      [Test]
      public void GetAsyncThrowsForUnknownId() {
        var exception =
          Assert.Throws<AggregateException>(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier).Result; });
        Assert.That(exception.InnerExceptions, Has.Count.EqualTo(1));
        Assert.That(exception.InnerExceptions[0], Is.InstanceOf<AggregateNotFoundException>());
        var actualException = (AggregateNotFoundException)exception.InnerExceptions[0];
        Assert.That(actualException.Identifier, Is.EqualTo(_model.UnknownIdentifier));
        Assert.That(actualException.Type, Is.EqualTo(typeof(AggregateRootEntityStub)));
      }

      [Test]
      public void GetResolvesNameOfUnknownId() {
        Catch.ExceptionOf(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier); });

        A.CallTo(() => _streamNameResolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetAsyncReturnsRootOfKnownId() {
        var result = _sut.GetAsync(_model.KnownIdentifier).Result;

        Assert.That(result, Is.SameAs(_root));
      }

      [Test]
      public void GetAsyncDoesNotResolveNameOfKnownId() {
        var _ = _sut.GetAsync(_model.KnownIdentifier).Result;

        A.CallTo(() => _streamNameResolver.Resolve(_model.KnownIdentifier)).MustNotHaveHappened();
      }

      [Test]
      public void GetOptionalAsyncReturnsEmptyForUnknownId() {
        var result = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

        Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void GetOptionalAsyncResolvesNameOfUnknownId() {
        var _ = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

        A.CallTo(() => _streamNameResolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalAsyncReturnsRootForKnownId() {
        var result = _sut.GetOptionalAsync(_model.KnownIdentifier).Result;

        Assert.That(result, Is.EqualTo(new Optional<AggregateRootEntityStub>(_root)));
      }

      [Test]
      public void GetOptionalAsyncDoesNotResolveNameOfKnownId() {
        var _ = _sut.GetOptionalAsync(_model.KnownIdentifier).Result;

        A.CallTo(() => _streamNameResolver.Resolve(_model.KnownIdentifier)).MustNotHaveHappened();
      }
    }

    [TestFixture]
    public class WithFilledStore {
      AsyncRepository<AggregateRootEntityStub> _sut;
      ConcurrentUnitOfWork _unitOfWork;
      AggregateRootEntityStub _root;
      Model _model;
      IStreamNameResolver _streamNameResolver;

      [SetUp]
      public void SetUp() {
        _model = new Model();
        using (var stream = new MemoryStream()) {
          using (var writer = new BinaryWriter(stream)) {
            new EventStub(1).Write(writer);
          }
          EmbeddedEventStore.Instance.Connection.AppendToStream(
            _model.KnownIdentifier,
            ExpectedVersion.NoStream,
            new EventData(
              Guid.NewGuid(),
              typeof(EventStub).AssemblyQualifiedName,
              false,
              stream.ToArray(),
              new byte[0]));
        }
        _root = AggregateRootEntityStub.Factory();
        _unitOfWork = new ConcurrentUnitOfWork();
        _streamNameResolver = A.Fake<IStreamNameResolver>();
        A.CallTo(() => _streamNameResolver.Resolve(_model.KnownIdentifier)).Returns(_model.KnownIdentifier);
        A.CallTo(() => _streamNameResolver.Resolve(_model.UnknownIdentifier)).Returns(_model.UnknownIdentifier);
        _sut = new AsyncRepository<AggregateRootEntityStub>(
          () => _root,
          _unitOfWork,
          EmbeddedEventStore.Instance.Connection,
          EventReaderConfigurationFactory.CreateWithResolver(_streamNameResolver));
      }

      [Test]
      public void GetAsyncThrowsForUnknownId() {
        var exception =
          Assert.Throws<AggregateException>(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier).Result; });
        Assert.That(exception.InnerExceptions, Has.Count.EqualTo(1));
        Assert.That(exception.InnerExceptions[0], Is.InstanceOf<AggregateNotFoundException>());
        var actualException = (AggregateNotFoundException)exception.InnerExceptions[0];
        Assert.That(actualException.Identifier, Is.EqualTo(_model.UnknownIdentifier));
        Assert.That(actualException.Type, Is.EqualTo(typeof(AggregateRootEntityStub)));
      }

      [Test]
      public void GetAsyncResolvesNameOfUnknownId() {
        Catch.ExceptionOf(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier).Result; });

        A.CallTo(() => _streamNameResolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetReturnsRootOfKnownId() {
        var result = _sut.GetAsync(_model.KnownIdentifier).Result;

        Assert.That(result, Is.SameAs(_root));
      }

      [Test]
      public void GetResolvesNameOfKnownId() {
        var _ = _sut.GetAsync(_model.KnownIdentifier).Result;

        A.CallTo(() => _streamNameResolver.Resolve(_model.KnownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalAsyncReturnsEmptyForUnknownId() {
        var result = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

        Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void GetOptionalAsyncResolvesNameOfUnknownId() {
        var _ = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

        A.CallTo(() => _streamNameResolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalAsyncReturnsRootForKnownId() {
        var result = _sut.GetOptionalAsync(_model.KnownIdentifier).Result;

        Assert.That(result, Is.EqualTo(new Optional<AggregateRootEntityStub>(_root)));
      }

      [Test]
      public void GetOptionalAsyncResolvesNameOfKnownId() {
        var _ = _sut.GetOptionalAsync(_model.KnownIdentifier).Result;

        A.CallTo(() => _streamNameResolver.Resolve(_model.KnownIdentifier)).MustHaveHappened();
      }
    }
  }
}
