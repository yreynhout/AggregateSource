using System;
using System.IO;
using AggregateSource.GEventStore.Framework;
using AggregateSource.GEventStore.Snapshots.Framework;
using EventStore.ClientAPI;
using FakeItEasy;
using NUnit.Framework;

namespace AggregateSource.GEventStore.Snapshots {
  namespace SnapshotableRepositoryTests {
    [TestFixture]
    public class Construction {
      ISnapshotReader _reader;
      EventStoreReadConfiguration _configuration;
      IEventStoreConnection _connection;
      UnitOfWork _unitOfWork;
      Func<SnapshotableAggregateRootEntityStub> _factory;

      [SetUp]
      public void SetUp() {
        _connection = EmbeddedEventStore.Instance.Connection;
        _reader = SnapshotReaderFactory.Create();
        _configuration = EventStoreReadConfigurationFactory.Create();
        _unitOfWork = new UnitOfWork();
        _factory = SnapshotableAggregateRootEntityStub.Factory;
      }

      [Test]
      public void FactoryCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new SnapshotableRepository<SnapshotableAggregateRootEntityStub>(null, _unitOfWork, _connection, _configuration, _reader));
      }

      [Test]
      public void UnitOfWorkCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new SnapshotableRepository<SnapshotableAggregateRootEntityStub>(_factory, null, _connection, _configuration, _reader));
      }

      [Test]
      public void EventStoreConnectionCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new SnapshotableRepository<SnapshotableAggregateRootEntityStub>(_factory, _unitOfWork, null, _configuration, _reader));
      }

      [Test]
      public void EventStoreReadConfigurationCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new SnapshotableRepository<SnapshotableAggregateRootEntityStub>(_factory, _unitOfWork, _connection, null, _reader));
      }

      [Test]
      public void SnapshotReaderCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new SnapshotableRepository<SnapshotableAggregateRootEntityStub>(_factory, _unitOfWork, _connection, _configuration, null));
      }
    }

    [TestFixture]
    public class WithEmptyStoreAndEmptyUnitOfWorkAndNoSnapshot {
      SnapshotableRepository<SnapshotableAggregateRootEntityStub> _sut;
      UnitOfWork _unitOfWork;
      Model _model;
      IStreamNameResolver _resolver;
      ISnapshotReader _reader;

      [SetUp]
      public void SetUp() {
        EmbeddedEventStore.Instance.Connection.DeleteAllStreams();
        _model = new Model();
        _unitOfWork = new UnitOfWork();
        _resolver = A.Fake<IStreamNameResolver>();
        _reader = A.Fake<ISnapshotReader>();
        A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).Returns(_model.KnownIdentifier);
        A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).Returns(_model.UnknownIdentifier);
        A.CallTo(() => _reader.ReadOptional(_model.KnownIdentifier)).Returns(Optional<Snapshot>.Empty);
        A.CallTo(() => _reader.ReadOptional(_model.UnknownIdentifier)).Returns(Optional<Snapshot>.Empty);
        _sut = new SnapshotableRepository<SnapshotableAggregateRootEntityStub>(
          SnapshotableAggregateRootEntityStub.Factory,
          _unitOfWork,
          EmbeddedEventStore.Instance.Connection,
          EventStoreReadConfigurationFactory.CreateWithResolver(_resolver),
          _reader);
      }

      [Test]
      public void GetThrows() {
        var exception =
          Assert.Throws<AggregateNotFoundException>(() => _sut.Get(_model.UnknownIdentifier));
        Assert.That(exception.Identifier, Is.EqualTo(_model.UnknownIdentifier));
        Assert.That(exception.Type, Is.EqualTo(typeof(SnapshotableAggregateRootEntityStub)));
      }

      [Test]
      public void GetReadsSnapshot() {
        Catch.ExceptionOf(() => _sut.Get(_model.UnknownIdentifier));
        
        A.CallTo(() => _reader.ReadOptional(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetResolvesStreamName() {
        Catch.ExceptionOf(() => _sut.Get(_model.UnknownIdentifier));

        A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalReturnsEmpty() {
        var result = _sut.GetOptional(_model.UnknownIdentifier);

        Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void GetOptionalReadsSnapshot() {
        var _ = _sut.GetOptional(_model.UnknownIdentifier);

        A.CallTo(() => _reader.ReadOptional(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalResolvesStreamName() {
        var _ = _sut.GetOptional(_model.UnknownIdentifier);

        A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void AddAttachesToUnitOfWork() {
        var root = SnapshotableAggregateRootEntityStub.Factory();

        _sut.Add(_model.KnownIdentifier, root);

        Aggregate aggregate;
        var result = _unitOfWork.TryGet(_model.KnownIdentifier, out aggregate);
        Assert.That(result, Is.True);
        Assert.That(aggregate.Identifier, Is.EqualTo(_model.KnownIdentifier));
        Assert.That(aggregate.Root, Is.SameAs(root));
      }
    }


    [TestFixture]
    public class WithEmptyStoreAndEmptyUnitOfWorkAndSnapshot {
      SnapshotableRepository<SnapshotableAggregateRootEntityStub> _sut;
      UnitOfWork _unitOfWork;
      Model _model;
      IStreamNameResolver _resolver;
      ISnapshotReader _reader;

      [SetUp]
      public void SetUp() {
        EmbeddedEventStore.Instance.Connection.DeleteAllStreams();
        _model = new Model();
        _unitOfWork = new UnitOfWork();
        _resolver = A.Fake<IStreamNameResolver>();
        _reader = A.Fake<ISnapshotReader>();
        A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).Returns(_model.KnownIdentifier);
        A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).Returns(_model.UnknownIdentifier);
        A.CallTo(() => _reader.ReadOptional(_model.KnownIdentifier)).Returns(new Optional<Snapshot>(new Snapshot(100, new object())));
        A.CallTo(() => _reader.ReadOptional(_model.UnknownIdentifier)).Returns(new Optional<Snapshot>(new Snapshot(100, new object())));
        _sut = new SnapshotableRepository<SnapshotableAggregateRootEntityStub>(
          SnapshotableAggregateRootEntityStub.Factory,
          _unitOfWork,
          EmbeddedEventStore.Instance.Connection,
          EventStoreReadConfigurationFactory.CreateWithResolver(_resolver),
          _reader);
      }

      [Test]
      public void GetThrows() {
        var exception =
          Assert.Throws<AggregateNotFoundException>(() => _sut.Get(_model.UnknownIdentifier));
        Assert.That(exception.Identifier, Is.EqualTo(_model.UnknownIdentifier));
        Assert.That(exception.Type, Is.EqualTo(typeof(SnapshotableAggregateRootEntityStub)));
      }

      [Test]
      public void GetReadsSnapshot() {
        Catch.ExceptionOf(() => _sut.Get(_model.UnknownIdentifier));

        A.CallTo(() => _reader.ReadOptional(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetResolvesStreamName() {
        Catch.ExceptionOf(() => _sut.Get(_model.UnknownIdentifier));

        A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalReturnsEmpty() {
        var result = _sut.GetOptional(_model.UnknownIdentifier);

        Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void GetOptionalReadsSnapshot() {
        var _ = _sut.GetOptional(_model.UnknownIdentifier);

        A.CallTo(() => _reader.ReadOptional(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalResolvesStreamName() {
        var _ = _sut.GetOptional(_model.UnknownIdentifier);

        A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void AddAttachesToUnitOfWork() {
        var root = SnapshotableAggregateRootEntityStub.Factory();

        _sut.Add(_model.KnownIdentifier, root);

        Aggregate aggregate;
        var result = _unitOfWork.TryGet(_model.KnownIdentifier, out aggregate);
        Assert.That(result, Is.True);
        Assert.That(aggregate.Identifier, Is.EqualTo(_model.KnownIdentifier));
        Assert.That(aggregate.Root, Is.SameAs(root));
      }
    }

    [TestFixture]
    public class WithEmptyStoreAndFilledUnitOfWorkAndNoSnapshot {
      SnapshotableRepository<SnapshotableAggregateRootEntityStub> _sut;
      UnitOfWork _unitOfWork;
      SnapshotableAggregateRootEntityStub _root;
      Model _model;
      IStreamNameResolver _resolver;
      ISnapshotReader _reader;

      [SetUp]
      public void SetUp() {
        EmbeddedEventStore.Instance.Connection.DeleteAllStreams();
        _model = new Model();
        _root = SnapshotableAggregateRootEntityStub.Factory();
        _unitOfWork = new UnitOfWork();
        _unitOfWork.Attach(new Aggregate(_model.KnownIdentifier, 0, _root));
        _resolver = A.Fake<IStreamNameResolver>();
        _reader = A.Fake<ISnapshotReader>();
        A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).Returns(_model.KnownIdentifier);
        A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).Returns(_model.UnknownIdentifier);
        A.CallTo(() => _reader.ReadOptional(_model.KnownIdentifier)).Returns(Optional<Snapshot>.Empty);
        A.CallTo(() => _reader.ReadOptional(_model.UnknownIdentifier)).Returns(Optional<Snapshot>.Empty);
        _sut = new SnapshotableRepository<SnapshotableAggregateRootEntityStub>(
          SnapshotableAggregateRootEntityStub.Factory,
          _unitOfWork,
          EmbeddedEventStore.Instance.Connection,
          EventStoreReadConfigurationFactory.CreateWithResolver(_resolver),
          _reader);
      }

      [Test]
      public void GetThrowsForUnknownId() {
        var exception =
          Assert.Throws<AggregateNotFoundException>(() => _sut.Get(_model.UnknownIdentifier));
        Assert.That(exception.Identifier, Is.EqualTo(_model.UnknownIdentifier));
        Assert.That(exception.Type, Is.EqualTo(typeof(SnapshotableAggregateRootEntityStub)));
      }

      [Test]
      public void GetReadsSnapshotOfUnknownId() {
        Catch.ExceptionOf(() => _sut.Get(_model.UnknownIdentifier));

        A.CallTo(() => _reader.ReadOptional(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetResolvesNameOfUnknownId() {
        Catch.ExceptionOf(() => _sut.Get(_model.UnknownIdentifier));

        A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetReturnsRootOfKnownId() {
        var result = _sut.Get(_model.KnownIdentifier);

        Assert.That(result, Is.SameAs(_root));
      }

      [Test]
      public void GetDoesNotReadSnapshotOfKnownId() {
        var _ = _sut.Get(_model.KnownIdentifier);

        A.CallTo(() => _reader.ReadOptional(_model.KnownIdentifier)).MustNotHaveHappened();
      }

      [Test]
      public void GetDoesNotResolveNameOfKnownId() {
        var _ = _sut.Get(_model.KnownIdentifier);

        A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).MustNotHaveHappened();
      }

      [Test]
      public void GetOptionalReturnsEmptyForUnknownId() {
        var result = _sut.GetOptional(_model.UnknownIdentifier);

        Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void GetOptionalReadsSnapshotOfUnknownId() {
        var _ = _sut.GetOptional(_model.UnknownIdentifier);

        A.CallTo(() => _reader.ReadOptional(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalResolvesNameOfUnknownId() {
        var _ = _sut.GetOptional(_model.UnknownIdentifier);

        A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalReturnsRootForKnownId() {
        var result = _sut.GetOptional(_model.KnownIdentifier);

        Assert.That(result, Is.EqualTo(new Optional<SnapshotableAggregateRootEntityStub>(_root)));
      }

      [Test]
      public void GetOptionalDoesNotReadSnapshotOfKnownId() {
        var _ = _sut.GetOptional(_model.KnownIdentifier);

        A.CallTo(() => _reader.ReadOptional(_model.KnownIdentifier)).MustNotHaveHappened();
      }

      [Test]
      public void GetOptionalDoesNotResolveNameOfKnownId() {
        var _ = _sut.GetOptional(_model.KnownIdentifier);

        A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).MustNotHaveHappened();
      }
    }

    [TestFixture]
    public class WithEmptyStoreAndFilledUnitOfWorkAndSnapshot {
      SnapshotableRepository<SnapshotableAggregateRootEntityStub> _sut;
      UnitOfWork _unitOfWork;
      SnapshotableAggregateRootEntityStub _root;
      Model _model;
      IStreamNameResolver _resolver;
      ISnapshotReader _reader;
      object _state;

      [SetUp]
      public void SetUp() {
        EmbeddedEventStore.Instance.Connection.DeleteAllStreams();
        _model = new Model();
        _root = SnapshotableAggregateRootEntityStub.Factory();
        _unitOfWork = new UnitOfWork();
        _unitOfWork.Attach(new Aggregate(_model.KnownIdentifier, 0, _root));
        _resolver = A.Fake<IStreamNameResolver>();
        _reader = A.Fake<ISnapshotReader>();
        _state = new object();
        A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).Returns(_model.KnownIdentifier);
        A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).Returns(_model.UnknownIdentifier);
        A.CallTo(() => _reader.ReadOptional(_model.KnownIdentifier)).Returns(new Optional<Snapshot>(new Snapshot(100, _state)));
        A.CallTo(() => _reader.ReadOptional(_model.UnknownIdentifier)).Returns(new Optional<Snapshot>(new Snapshot(100, _state)));
        _sut = new SnapshotableRepository<SnapshotableAggregateRootEntityStub>(
          SnapshotableAggregateRootEntityStub.Factory,
          _unitOfWork,
          EmbeddedEventStore.Instance.Connection,
          EventStoreReadConfigurationFactory.CreateWithResolver(_resolver),
          _reader);
      }

      [Test]
      public void GetThrowsForUnknownId() {
        var exception =
          Assert.Throws<AggregateNotFoundException>(() => _sut.Get(_model.UnknownIdentifier));
        Assert.That(exception.Identifier, Is.EqualTo(_model.UnknownIdentifier));
        Assert.That(exception.Type, Is.EqualTo(typeof(SnapshotableAggregateRootEntityStub)));
      }

      [Test]
      public void GetReadsSnapshotOfUnknownId() {
        Catch.ExceptionOf(() => _sut.Get(_model.UnknownIdentifier));

        A.CallTo(() => _reader.ReadOptional(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetResolvesNameOfUnknownId() {
        Catch.ExceptionOf(() => _sut.Get(_model.UnknownIdentifier));

        A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetReturnsRootOfKnownId() {
        var result = _sut.Get(_model.KnownIdentifier);

        Assert.That(result, Is.SameAs(_root));
      }

      [Test]
      public void GetReturnsRootOfKnownIdNotRestoredFromSnapshot() {
        var result = _sut.Get(_model.KnownIdentifier);

        Assert.That(result.RestoredSnapshot, Is.Null);
      }

      [Test]
      public void GetDoesNotReadSnapshotOfKnownId() {
        var _ = _sut.Get(_model.KnownIdentifier);

        A.CallTo(() => _reader.ReadOptional(_model.KnownIdentifier)).MustNotHaveHappened();
      }

      [Test]
      public void GetDoesNotResolveNameOfKnownId() {
        var _ = _sut.Get(_model.KnownIdentifier);

        A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).MustNotHaveHappened();
      }

      [Test]
      public void GetOptionalReturnsEmptyForUnknownId() {
        var result = _sut.GetOptional(_model.UnknownIdentifier);

        Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void GetOptionalReadsSnapshotOfUnknownId() {
        var _ = _sut.GetOptional(_model.UnknownIdentifier);

        A.CallTo(() => _reader.ReadOptional(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalResolvesNameOfUnknownId() {
        var _ = _sut.GetOptional(_model.UnknownIdentifier);

        A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalReturnsRootForKnownId() {
        var result = _sut.GetOptional(_model.KnownIdentifier);

        Assert.That(result, Is.EqualTo(new Optional<SnapshotableAggregateRootEntityStub>(_root)));
      }

      [Test]
      public void GetOptionalReturnsRootForKnownIdNotRestoredFromSnapshot() {
        var result = _sut.GetOptional(_model.KnownIdentifier);

        Assert.That(result.Value.RestoredSnapshot, Is.Null);
      }

      [Test]
      public void GetOptionalDoesNotReadSnapshotOfKnownId() {
        var _ = _sut.GetOptional(_model.KnownIdentifier);

        A.CallTo(() => _reader.ReadOptional(_model.KnownIdentifier)).MustNotHaveHappened();
      }

      [Test]
      public void GetOptionalDoesNotResolveNameOfKnownId() {
        var _ = _sut.GetOptional(_model.KnownIdentifier);

        A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).MustNotHaveHappened();
      }
    }

    [TestFixture]
    public class WithFilledStoreAndNoSnapshot {
      SnapshotableRepository<SnapshotableAggregateRootEntityStub> _sut;
      UnitOfWork _unitOfWork;
      SnapshotableAggregateRootEntityStub _root;
      Model _model;
      IStreamNameResolver _resolver;
      ISnapshotReader _reader;

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
              typeof(Event).AssemblyQualifiedName,
              false,
              stream.ToArray(),
              new byte[0]));
        }
        _root = SnapshotableAggregateRootEntityStub.Factory();
        _unitOfWork = new UnitOfWork();
        _resolver = A.Fake<IStreamNameResolver>();
        _reader = A.Fake<ISnapshotReader>();
        A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).Returns(_model.KnownIdentifier);
        A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).Returns(_model.UnknownIdentifier);
        A.CallTo(() => _reader.ReadOptional(_model.KnownIdentifier)).Returns(Optional<Snapshot>.Empty);
        A.CallTo(() => _reader.ReadOptional(_model.UnknownIdentifier)).Returns(Optional<Snapshot>.Empty);
        _sut = new SnapshotableRepository<SnapshotableAggregateRootEntityStub>(
          () => _root,
          _unitOfWork,
          EmbeddedEventStore.Instance.Connection,
          EventStoreReadConfigurationFactory.CreateWithResolver(_resolver),
          _reader);
      }

      [Test]
      public void GetThrowsForUnknownId() {
        var exception =
          Assert.Throws<AggregateNotFoundException>(() => _sut.Get(_model.UnknownIdentifier));
        Assert.That(exception.Identifier, Is.EqualTo(_model.UnknownIdentifier));
        Assert.That(exception.Type, Is.EqualTo(typeof(SnapshotableAggregateRootEntityStub)));
      }

      [Test]
      public void GetReadsSnapshotOfUnknownId() {
        Catch.ExceptionOf(() => _sut.Get(_model.UnknownIdentifier));

        A.CallTo(() => _reader.ReadOptional(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetResolvesNameOfUnknownId() {
        Catch.ExceptionOf(() => _sut.Get(_model.UnknownIdentifier));

        A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetReturnsRootOfKnownId() {
        var result = _sut.Get(_model.KnownIdentifier);

        Assert.That(result, Is.SameAs(_root));
      }

      [Test]
      public void GetReadsSnapshotOfKnownId() {
        var _ = _sut.Get(_model.KnownIdentifier);

        A.CallTo(() => _reader.ReadOptional(_model.KnownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetResolvesNameOfKnownId() {
        var _ = _sut.Get(_model.KnownIdentifier);

        A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalReturnsEmptyForUnknownId() {
        var result = _sut.GetOptional(_model.UnknownIdentifier);

        Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void GetOptionalReadsSnapshotOfUnknownId() {
        var _ = _sut.GetOptional(_model.UnknownIdentifier);

        A.CallTo(() => _reader.ReadOptional(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalResolvesNameOfUnknownId() {
        var _ = _sut.GetOptional(_model.UnknownIdentifier);

        A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalReturnsRootForKnownId() {
        var result = _sut.GetOptional(_model.KnownIdentifier);

        Assert.That(result, Is.EqualTo(new Optional<SnapshotableAggregateRootEntityStub>(_root)));
      }

      [Test]
      public void GetOptionalReadsSnapshotOfKnownId() {
        var _ = _sut.GetOptional(_model.KnownIdentifier);

        A.CallTo(() => _reader.ReadOptional(_model.KnownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalResolvesNameOfKnownId() {
        var _ = _sut.GetOptional(_model.KnownIdentifier);

        A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).MustHaveHappened();
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

    [TestFixture]
    public class WithFilledStoreAndSnapshot {
      SnapshotableRepository<SnapshotableAggregateRootEntityStub> _sut;
      UnitOfWork _unitOfWork;
      SnapshotableAggregateRootEntityStub _root;
      Model _model;
      IStreamNameResolver _resolver;
      ISnapshotReader _reader;
      object _state;

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
              typeof(Event).AssemblyQualifiedName,
              false,
              stream.ToArray(),
              new byte[0]));
          EmbeddedEventStore.Instance.Connection.AppendToStream(
            _model.KnownIdentifier,
            1,
            new EventData(
              Guid.NewGuid(),
              typeof(Event).AssemblyQualifiedName,
              false,
              stream.ToArray(),
              new byte[0]));
        }
        _root = SnapshotableAggregateRootEntityStub.Factory();
        _state = new object();
        _unitOfWork = new UnitOfWork();
        _resolver = A.Fake<IStreamNameResolver>();
        _reader = A.Fake<ISnapshotReader>();
        A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).Returns(_model.KnownIdentifier);
        A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).Returns(_model.UnknownIdentifier);
        A.CallTo(() => _reader.ReadOptional(_model.KnownIdentifier)).Returns(new Optional<Snapshot>(new Snapshot(1, _state)));
        A.CallTo(() => _reader.ReadOptional(_model.UnknownIdentifier)).Returns(new Optional<Snapshot>(new Snapshot(1, _state)));
        _sut = new SnapshotableRepository<SnapshotableAggregateRootEntityStub>(
          () => _root,
          _unitOfWork,
          EmbeddedEventStore.Instance.Connection,
          EventStoreReadConfigurationFactory.CreateWithResolver(_resolver),
          _reader);
      }

      [Test]
      public void GetThrowsForUnknownId() {
        var exception =
          Assert.Throws<AggregateNotFoundException>(() => _sut.Get(_model.UnknownIdentifier));
        Assert.That(exception.Identifier, Is.EqualTo(_model.UnknownIdentifier));
        Assert.That(exception.Type, Is.EqualTo(typeof(SnapshotableAggregateRootEntityStub)));
      }

      [Test]
      public void GetReadsSnapshotOfUnknownId() {
        Catch.ExceptionOf(() => _sut.Get(_model.UnknownIdentifier));

        A.CallTo(() => _reader.ReadOptional(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetResolvesNameOfUnknownId() {
        Catch.ExceptionOf(() => _sut.Get(_model.UnknownIdentifier));

        A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetReturnsRootOfKnownId() {
        var result = _sut.Get(_model.KnownIdentifier);

        Assert.That(result, Is.SameAs(_root));
      }

      [Test]
      public void GetReturnsRootRestoredFromSnapshot() {
        var result = _sut.Get(_model.KnownIdentifier);

        Assert.That(result.RestoredSnapshot, Is.SameAs(_state));
      }

      [Test]
      public void GetReadsSnapshotOfKnownId() {
        var _ = _sut.Get(_model.KnownIdentifier);

        A.CallTo(() => _reader.ReadOptional(_model.KnownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetResolvesNameOfKnownId() {
        var _ = _sut.Get(_model.KnownIdentifier);

        A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalReturnsEmptyForUnknownId() {
        var result = _sut.GetOptional(_model.UnknownIdentifier);

        Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void GetOptionalReadsSnapshotOfUnknownId() {
        var _ = _sut.GetOptional(_model.UnknownIdentifier);

        A.CallTo(() => _reader.ReadOptional(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalResolvesNameOfUnknownId() {
        var _ = _sut.GetOptional(_model.UnknownIdentifier);

        A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalReturnsRootForKnownId() {
        var result = _sut.GetOptional(_model.KnownIdentifier);

        Assert.That(result, Is.EqualTo(new Optional<SnapshotableAggregateRootEntityStub>(_root)));
      }

      [Test]
      public void GetOptionalReturnsRootForKnownIdRestoredFromSnapshot() {
        var result = _sut.GetOptional(_model.KnownIdentifier);

        Assert.That(result.Value.RestoredSnapshot, Is.SameAs(_state));
      }

      [Test]
      public void GetOptionalReadsSnapshotOfKnownId() {
        var _ = _sut.GetOptional(_model.KnownIdentifier);

        A.CallTo(() => _reader.ReadOptional(_model.KnownIdentifier)).MustHaveHappened();
      }

      [Test]
      public void GetOptionalResolvesNameOfKnownId() {
        var _ = _sut.GetOptional(_model.KnownIdentifier);

        A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).MustHaveHappened();
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