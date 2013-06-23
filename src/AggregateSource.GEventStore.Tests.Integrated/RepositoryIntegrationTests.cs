using System;
using System.IO;
using AggregateSource.GEventStore.Framework;
using EventStore.ClientAPI;
using FakeItEasy;
using NUnit.Framework;

namespace AggregateSource.GEventStore {
  namespace RepositoryIntegrationTests {
    [TestFixture]
    public class Construction {
      [Test]
      public void FactoryCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new Repository<AggregateRootEntityStub>(null, new UnitOfWork(), EmbeddedEventStore.Instance.Connection, EventStoreReadConfigurationFactory.NewInstance()));
      }

      [Test]
      public void UnitOfWorkCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new Repository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, null, EmbeddedEventStore.Instance.Connection, EventStoreReadConfigurationFactory.NewInstance()));
      }

      [Test]
      public void EventStoreConnectionCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new Repository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, new UnitOfWork(), null, EventStoreReadConfigurationFactory.NewInstance()));
      }

      [Test]
      public void EventStoreReadConfigurationCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new Repository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, new UnitOfWork(), EmbeddedEventStore.Instance.Connection, null));
      }
    }

    [TestFixture]
    public class WithEmptyStoreAndEmptyUnitOfWork {
      Repository<AggregateRootEntityStub> _sut;
      UnitOfWork _unitOfWork;
      Model _model;
      IStreamNameResolver _streamNameResolver;

      [SetUp]
      public void SetUp() {
        EmbeddedEventStore.Instance.Connection.DeleteAllStreams();
        _model = new Model();
        _unitOfWork = new UnitOfWork();
        _streamNameResolver = A.Fake<IStreamNameResolver>();
        A.CallTo(() => _streamNameResolver.Resolve(_model.KnownIdentifier)).Returns(_model.KnownIdentifier);
        A.CallTo(() => _streamNameResolver.Resolve(_model.UnknownIdentifier)).Returns(_model.UnknownIdentifier);
        _sut = new Repository<AggregateRootEntityStub>(
          AggregateRootEntityStub.Factory, 
          _unitOfWork, 
          EmbeddedEventStore.Instance.Connection, 
          EventStoreReadConfigurationFactory.NewInstanceWithStreamNameResolver(_streamNameResolver));
      }

      [Test]
      public void GetThrows() {
        var exception =
          Assert.Throws<AggregateNotFoundException>(() => _sut.Get(_model.UnknownIdentifier));
        Assert.That(exception.Identifier, Is.EqualTo(_model.UnknownIdentifier));
        Assert.That(exception.Type, Is.EqualTo(typeof(AggregateRootEntityStub)));
      }

      [Test]
      public void GetResolvesStreamName() {
        Catch.ExceptionOf(() => _sut.Get(_model.UnknownIdentifier));

        A.CallTo(() => _streamNameResolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalReturnsEmpty() {
        var result = _sut.GetOptional(_model.UnknownIdentifier);

        Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void GetOptionalResolvesStreamName() {
        var _ = _sut.GetOptional(_model.UnknownIdentifier);

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
      Repository<AggregateRootEntityStub> _sut;
      UnitOfWork _unitOfWork;
      AggregateRootEntityStub _root;
      Model _model;
      IStreamNameResolver _streamNameResolver;

      [SetUp]
      public void SetUp() {
        EmbeddedEventStore.Instance.Connection.DeleteAllStreams();
        _model = new Model();
        _root = AggregateRootEntityStub.Factory();
        _unitOfWork = new UnitOfWork();
        _unitOfWork.Attach(new Aggregate(_model.KnownIdentifier, 0, _root));
        _streamNameResolver = A.Fake<IStreamNameResolver>();
        A.CallTo(() => _streamNameResolver.Resolve(_model.KnownIdentifier)).Returns(_model.KnownIdentifier);
        A.CallTo(() => _streamNameResolver.Resolve(_model.UnknownIdentifier)).Returns(_model.UnknownIdentifier);
        _sut = new Repository<AggregateRootEntityStub>(
          AggregateRootEntityStub.Factory, 
          _unitOfWork, 
          EmbeddedEventStore.Instance.Connection, 
          EventStoreReadConfigurationFactory.NewInstanceWithStreamNameResolver(_streamNameResolver));
      }

      [Test]
      public void GetThrowsForUnknownId() {
        var exception =
          Assert.Throws<AggregateNotFoundException>(() => _sut.Get(_model.UnknownIdentifier));
        Assert.That(exception.Identifier, Is.EqualTo(_model.UnknownIdentifier));
        Assert.That(exception.Type, Is.EqualTo(typeof(AggregateRootEntityStub)));
      }

      [Test]
      public void GetResolvesNameOfUnknownId() {
        Catch.ExceptionOf(() => _sut.Get(_model.UnknownIdentifier));

        A.CallTo(() => _streamNameResolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetReturnsRootOfKnownId() {
        var result = _sut.Get(_model.KnownIdentifier);

        Assert.That(result, Is.SameAs(_root));
      }

      [Test]
      public void GetDoesNotResolveNameOfKnownId() {
        var _ = _sut.Get(_model.KnownIdentifier);

        A.CallTo(() => _streamNameResolver.Resolve(_model.KnownIdentifier)).MustNotHaveHappened();
      }

      [Test]
      public void GetOptionalReturnsEmptyForUnknownId() {
        var result = _sut.GetOptional(_model.UnknownIdentifier);

        Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void GetOptionalResolvesNameOfUnknownId() {
        var _ = _sut.GetOptional(_model.UnknownIdentifier);

        A.CallTo(() => _streamNameResolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalReturnsRootForKnownId() {
        var result = _sut.GetOptional(_model.KnownIdentifier);

        Assert.That(result, Is.EqualTo(new Optional<AggregateRootEntityStub>(_root)));
      }

      [Test]
      public void GetOptionalDoesNotResolveNameOfKnownId() {
        var _ = _sut.GetOptional(_model.KnownIdentifier);

        A.CallTo(() => _streamNameResolver.Resolve(_model.KnownIdentifier)).MustNotHaveHappened();
      }
    }

    [TestFixture]
    public class WithFilledStore {
      Repository<AggregateRootEntityStub> _sut;
      UnitOfWork _unitOfWork;
      AggregateRootEntityStub _root;
      Model _model;
      IStreamNameResolver _streamNameResolver;

      [SetUp]
      public void SetUp() {
        _model = new Model();
        using (var stream = new MemoryStream()) {
          using (var writer = new BinaryWriter(stream)) {
            new Event().Write(writer);
          }
          EmbeddedEventStore.Instance.Connection.AppendToStream(
            _model.KnownIdentifier,
            ExpectedVersion.NoStream,
            new EventData(
              Guid.NewGuid(),
              typeof (Event).AssemblyQualifiedName,
              false,
              stream.ToArray(),
              new byte[0]));
        }
        _root = AggregateRootEntityStub.Factory();
        _unitOfWork = new UnitOfWork();
        _streamNameResolver = A.Fake<IStreamNameResolver>();
        A.CallTo(() => _streamNameResolver.Resolve(_model.KnownIdentifier)).Returns(_model.KnownIdentifier);
        A.CallTo(() => _streamNameResolver.Resolve(_model.UnknownIdentifier)).Returns(_model.UnknownIdentifier);
        _sut = new Repository<AggregateRootEntityStub>(
          () => _root,
          _unitOfWork,
          EmbeddedEventStore.Instance.Connection,
          EventStoreReadConfigurationFactory.NewInstanceWithStreamNameResolver(_streamNameResolver));
      }

      [Test]
      public void GetThrowsForUnknownId() {
        var exception =
          Assert.Throws<AggregateNotFoundException>(() => _sut.Get(_model.UnknownIdentifier));
        Assert.That(exception.Identifier, Is.EqualTo(_model.UnknownIdentifier));
        Assert.That(exception.Type, Is.EqualTo(typeof(AggregateRootEntityStub)));
      }

      [Test]
      public void GetResolvesNameOfUnknownId() {
        Catch.ExceptionOf(() => _sut.Get(_model.UnknownIdentifier));

        A.CallTo(() => _streamNameResolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetReturnsRootOfKnownId() {
        var result = _sut.Get(_model.KnownIdentifier);

        Assert.That(result, Is.SameAs(_root));
      }

      [Test]
      public void GetResolvesNameOfKnownId() {
        var _ = _sut.Get(_model.KnownIdentifier);

        A.CallTo(() => _streamNameResolver.Resolve(_model.KnownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalReturnsEmptyForUnknownId() {
        var result = _sut.GetOptional(_model.UnknownIdentifier);

        Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void GetOptionalResolvesNameOfUnknownId() {
        var _ = _sut.GetOptional(_model.UnknownIdentifier);

        A.CallTo(() => _streamNameResolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalReturnsRootForKnownId() {
        var result = _sut.GetOptional(_model.KnownIdentifier);

        Assert.That(result, Is.EqualTo(new Optional<AggregateRootEntityStub>(_root)));
      }

      [Test]
      public void GetOptionalResolvesNameOfKnownId() {
        var _ = _sut.GetOptional(_model.KnownIdentifier);

        A.CallTo(() => _streamNameResolver.Resolve(_model.KnownIdentifier)).MustHaveHappened();
      }

      class Event : IBinarySerializer, IBinaryDeserializer {
        public void Write(BinaryWriter writer) {
          writer.Write(true);
        }

        public void Read(BinaryReader reader) {
          reader.ReadBoolean();
        }
      }
    }
  }
}
