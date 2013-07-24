using System;
using System.IO;
using AggregateSource.GEventStore.Framework;
using AggregateSource.GEventStore.Framework.Snapshots;
using EventStore.ClientAPI;
using NUnit.Framework;

namespace AggregateSource.GEventStore.Snapshots {
  namespace SnapshotReaderTests {
    [TestFixture]
    public class WithAnyInstance {
      SnapshotReaderConfiguration _configuration;
      IEventStoreConnection _connection;

      [SetUp]
      public void SetUp() {
        _configuration = SnapshotReaderConfigurationFactory.Create();
        _connection = EmbeddedEventStore.Instance.Connection;
      }

      [Test]
      public void ConnectionCannotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new SnapshotReader(null, _configuration));
      }

      [Test]
      public void ConfigurationCannotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new SnapshotReader(_connection, null));
      }

      [Test]
      public void IsSnapshotReader() {
        Assert.That(SnapshotReaderFactory.Create(), Is.InstanceOf<ISnapshotReader>());
      }

      [Test]
      public void ConfigurationReturnsExpectedValue() {
        var configuration = SnapshotReaderConfigurationFactory.Create();
        Assert.That(SnapshotReaderFactory.CreateWithConfiguration(configuration).Configuration, Is.SameAs(configuration));
      }

      [Test]
      public void ConnectionReturnsExpectedValue() {
        var connection = EmbeddedEventStore.Instance.Connection;
        Assert.That(SnapshotReaderFactory.CreateWithConnection(connection).Connection, Is.SameAs(connection));
      }

      [Test]
      public void ReadIdentifierCannotBeNull() {
        var sut = SnapshotReaderFactory.Create();
        Assert.Throws<ArgumentNullException>(() => sut.ReadOptional(null));
      }
    }

    [TestFixture]
    public class WithSnapshotStreamFoundInStore {
      Model _model;
      SnapshotReader _sut;

      [SetUp]
      public void SetUp() {
        _model = new Model();
        _sut = SnapshotReaderFactory.Create();
        CreateSnapshotStreamWithOneSnapshot(_sut.Configuration.StreamNameResolver.Resolve(_model.KnownIdentifier));
      }

      static void CreateSnapshotStreamWithOneSnapshot(string snapshotStreamName) {
        using (var stream = new MemoryStream()) {
          using (var writer = new BinaryWriter(stream)) {
            new SnapshotStateStub(1).Write(writer);
          }
          EmbeddedEventStore.Instance.Connection.AppendToStream(
            snapshotStreamName,
            ExpectedVersion.NoStream,
            new EventData(
              Guid.NewGuid(),
              typeof (SnapshotStateStub).AssemblyQualifiedName,
              false,
              stream.ToArray(),
              BitConverter.GetBytes(100)));
        }
      }

      [Test]
      public void GetReturnsSnapshotOfKnownId() {
        var result = _sut.ReadOptional(_model.KnownIdentifier);

        Assert.That(result, Is.EqualTo(new Optional<Snapshot>(new Snapshot(100, new SnapshotStateStub(1)))));
      }

      [Test]
      public void GetReturnsEmptyForUnknownId() {
        var result = _sut.ReadOptional(_model.UnknownIdentifier);

        Assert.That(result, Is.EqualTo(Optional<Snapshot>.Empty));
      }
    }

    [TestFixture]
    public class WithSnapshotStreamNotFoundInStore {
      Model _model;
      SnapshotReader _sut;

      [SetUp]
      public void SetUp() {
        _model = new Model();
        _sut = SnapshotReaderFactory.Create();
      }

      [Test]
      public void GetReturnsEmptyForKnownId() {
        var result = _sut.ReadOptional(_model.KnownIdentifier);

        Assert.That(result, Is.EqualTo(Optional<Snapshot>.Empty));
      }
    }

    [TestFixture]
    public class WithEmptySnapshotStreamInStore {
      Model _model;
      SnapshotReader _sut;

      [SetUp]
      public void SetUp() {
        _model = new Model();
        _sut = SnapshotReaderFactory.Create();
        CreateEmptySnapshotStream(_sut.Configuration.StreamNameResolver.Resolve(_model.KnownIdentifier));
      }

      static void CreateEmptySnapshotStream(string snapshotStreamName) {
        EmbeddedEventStore.Instance.Connection.AppendToStream(
          snapshotStreamName,
          ExpectedVersion.Any,
          new EventData[0]);
      }

      [Test]
      public void GetReturnsEmptyForKnownId() {
        var result = _sut.ReadOptional(_model.KnownIdentifier);

        Assert.That(result, Is.EqualTo(Optional<Snapshot>.Empty));
      }

      [Test]
      public void GetReturnsEmptyForUnknownId() {
        var result = _sut.ReadOptional(_model.UnknownIdentifier);

        Assert.That(result, Is.EqualTo(Optional<Snapshot>.Empty));
      }
    }

    [TestFixture]
    public class WithDeletedSnapshotStreamInStore {
      Model _model;
      SnapshotReader _sut;

      [SetUp]
      public void SetUp() {
        _model = new Model();
        _sut = SnapshotReaderFactory.Create();
        CreateDeletedSnapshotStream(_sut.Configuration.StreamNameResolver.Resolve(_model.KnownIdentifier));
      }

      static void CreateDeletedSnapshotStream(string snapshotStreamName) {
        EmbeddedEventStore.Instance.Connection.AppendToStream(
          snapshotStreamName,
          ExpectedVersion.Any,
          new EventData[0]);
        EmbeddedEventStore.Instance.Connection.DeleteStream(
          snapshotStreamName,
          ExpectedVersion.EmptyStream);
      }

      [Test]
      public void GetReturnsSnapshotOfKnownId() {
        var result = _sut.ReadOptional(_model.KnownIdentifier);

        Assert.That(result, Is.EqualTo(Optional<Snapshot>.Empty));
      }

      [Test]
      public void GetReturnsEmptyForUnknownId() {
        var result = _sut.ReadOptional(_model.UnknownIdentifier);

        Assert.That(result, Is.EqualTo(Optional<Snapshot>.Empty));
      }
    }
  }
}
