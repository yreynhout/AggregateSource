using System;
using System.IO;
using EventStore.ClientAPI;
using NUnit.Framework;
using ProtoBuf;

namespace AggregateSource.GEventStore {
  namespace RepositoryIntegrationTests {
    [TestFixture]
    public class Construction {
      [Test]
      public void FactoryCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() => 
          new Repository<AggregateRootEntityStub>(null, new UnitOfWork(), EmbeddedEventStore.Instance.Connection));
      }

      [Test]
      public void UnitOfWorkCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() => 
          new Repository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, null, EmbeddedEventStore.Instance.Connection));
      }

      [Test]
      public void EventStoreConnectionCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new Repository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, new UnitOfWork(), null));
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

      [SetUp]
      public void SetUp() {
        EmbeddedEventStore.Instance.Connection.DeleteAllStreams();
        _unitOfWork = new UnitOfWork();
        _sut = new Repository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, _unitOfWork, EmbeddedEventStore.Instance.Connection);
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
        EmbeddedEventStore.Instance.Connection.DeleteAllStreams();
        _id = Guid.NewGuid();
        _root = AggregateRootEntityStub.Factory();
        _unitOfWork = new UnitOfWork();
        _unitOfWork.Attach(new Aggregate(_id, 0, _root));
        _sut = new Repository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, _unitOfWork, EmbeddedEventStore.Instance.Connection);
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
        using (var stream = new MemoryStream()) {
          Serializer.Serialize(stream, new Event());
          EmbeddedEventStore.Instance.Connection.AppendToStream(
            StreamName.Create<AggregateRootEntityStub>(_id),
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
        _sut = new Repository<AggregateRootEntityStub>(
          () => _root,
          _unitOfWork,
          EmbeddedEventStore.Instance.Connection);
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

      [ProtoContract]
      class Event {}
    }
  }
}
