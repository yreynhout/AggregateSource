using System;
using System.IO;
using AggregateSource.GEventStore.Framework;
using EventStore.ClientAPI;
using FakeItEasy;
using NUnit.Framework;

namespace AggregateSource.GEventStore.Snapshots {
  namespace SnapshotReaderTests {
    [TestFixture]
    public class WithAnyInstance {
      SnapshotStoreReadConfiguration _configuration;

      [SetUp]
      public void SetUp() {
        _configuration = new SnapshotStoreReadConfiguration(
          A.Fake<IStreamNameResolver>(),
          A.Fake<ISnapshotDeserializer>());
      }

      [Test]
      public void ConnectionCannotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new SnapshotReader(null, _configuration));
      }

      [Test]
      public void ConfigurationCannotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new SnapshotReader(EmbeddedEventStore.Instance.Connection, null));
      }

      [Test]
      public void IsSnapshotReader() {
        Assert.That(new SnapshotReader(EmbeddedEventStore.Instance.Connection, _configuration), Is.InstanceOf<ISnapshotReader>());
      }

      [Test]
      public void ReadIdentifierCannotBeNull() {
        var sut = SutFactory.Create();
        Assert.Throws<ArgumentNullException>(() => sut.ReadOptional(null));
      }
    }

    [TestFixture]
    public class WithSnapshotStreamFoundInStore {
      Model _model;
      SnapshotStoreReadConfiguration _configuration;
      SnapshotReader _sut;

      [SetUp]
      public void SetUp() {
        _model = new Model();
        _configuration = 
          new SnapshotStoreReadConfiguration(
            new SnapshotStreamNameResolver(), 
            new SnapshotDeserializer());

        CreateSnapshotStreamWithOneSnapshot(_configuration.Resolver.Resolve(_model.KnownIdentifier));

        _sut = SutFactory.Create(_configuration);
      }

      static void CreateSnapshotStreamWithOneSnapshot(string snapshotStreamName) {
        using (var stream = new MemoryStream()) {
          using (var writer = new BinaryWriter(stream)) {
            new SnapshotState().Write(writer);
          }
          EmbeddedEventStore.Instance.Connection.AppendToStream(
            snapshotStreamName,
            ExpectedVersion.NoStream,
            new EventData(
              Guid.NewGuid(),
              typeof (SnapshotState).AssemblyQualifiedName,
              false,
              stream.ToArray(),
              BitConverter.GetBytes(100)));
        }
      }

      [Test]
      public void GetReturnsSnapshotOfKnownId() {
        var result = _sut.ReadOptional(_model.KnownIdentifier);

        Assert.That(result, Is.EqualTo(new Optional<Snapshot>(new Snapshot(100, new SnapshotState()))));
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
        _sut = SutFactory.Create();
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
      SnapshotStoreReadConfiguration _configuration;
      SnapshotReader _sut;

      [SetUp]
      public void SetUp() {
        _model = new Model();
        _configuration =
          new SnapshotStoreReadConfiguration(
            new SnapshotStreamNameResolver(),
            new SnapshotDeserializer());

        CreateEmptySnapshotStream(_configuration.Resolver.Resolve(_model.KnownIdentifier));

        _sut = SutFactory.Create(_configuration);
      }

      static void CreateEmptySnapshotStream(string snapshotStreamName) {
        EmbeddedEventStore.Instance.Connection.CreateStream(
          snapshotStreamName,
          Guid.NewGuid(),
          false,
          new byte[0]);
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
      SnapshotStoreReadConfiguration _configuration;
      SnapshotReader _sut;

      [SetUp]
      public void SetUp() {
        _model = new Model();
        _configuration =
          new SnapshotStoreReadConfiguration(
            new SnapshotStreamNameResolver(),
            new SnapshotDeserializer());

        CreateDeletedSnapshotStream(_configuration.Resolver.Resolve(_model.KnownIdentifier));

        _sut = SutFactory.Create(_configuration);
      }

      static void CreateDeletedSnapshotStream(string snapshotStreamName) {
        EmbeddedEventStore.Instance.Connection.CreateStream(
          snapshotStreamName,
          Guid.NewGuid(),
          false,
          new byte[0]);
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

    class SutFactory {
      public static SnapshotReader Create() {
        return Create(new SnapshotStoreReadConfiguration(new SnapshotStreamNameResolver(), new SnapshotDeserializer()));
      }

      public static SnapshotReader Create(SnapshotStoreReadConfiguration configuration) {
        return new SnapshotReader(EmbeddedEventStore.Instance.Connection, configuration);
      }
    }

    class SnapshotStreamNameResolver : IStreamNameResolver {
      public string Resolve(string identifier) {
        return identifier + "-snapshot";
      }
    }

    class SnapshotDeserializer : ISnapshotDeserializer {
      public Snapshot Deserialize(ResolvedEvent resolvedEvent) {
        var type = Type.GetType(resolvedEvent.Event.EventType, true);
        var instance = Activator.CreateInstance(type);
        using (var stream = new MemoryStream(resolvedEvent.Event.Data)) {
          using (var reader = new BinaryReader(stream)) {
            ((IBinaryDeserializer) instance).Read(reader);
            return new Snapshot(BitConverter.ToInt32(resolvedEvent.Event.Metadata, 0), instance);
          }
        }
        
      }
    }

    class SnapshotState : IBinarySerializer, IBinaryDeserializer {
      string _value;

      public SnapshotState() {
        _value = "snapshot";
      }

      public void Write(BinaryWriter writer) {
        writer.Write(_value);
      }

      public void Read(BinaryReader reader) {
        _value = reader.ReadString();
      }

      public override bool Equals(object obj) {
        return Equals(obj as SnapshotState);
      }

      bool Equals(SnapshotState other) {
        return !ReferenceEquals(other, null) && _value.Equals(other._value);
      }

      public override int GetHashCode() {
        return _value.GetHashCode();
      }
    }
  }
}
