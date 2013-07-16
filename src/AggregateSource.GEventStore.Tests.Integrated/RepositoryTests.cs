using System;
using AggregateSource.GEventStore.Framework;
using NUnit.Framework;

namespace AggregateSource.GEventStore {
  namespace RepositoryTests {
    [TestFixture]
    public class Construction {
      [Test]
      public void FactoryCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new Repository<AggregateRootEntityStub>(null, new UnitOfWork(), EmbeddedEventStore.Instance.Connection, EventReaderConfigurationFactory.Create()));
      }

      [Test]
      public void UnitOfWorkCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new Repository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, null, EmbeddedEventStore.Instance.Connection, EventReaderConfigurationFactory.Create()));
      }

      [Test]
      public void EventStoreConnectionCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new Repository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, new UnitOfWork(), null, EventReaderConfigurationFactory.Create()));
      }

      [Test]
      public void EventReaderConfigurationCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() =>
          new Repository<AggregateRootEntityStub>(AggregateRootEntityStub.Factory, new UnitOfWork(), EmbeddedEventStore.Instance.Connection, null));
      }

      [Ignore("TODO after merge - requires setup")]
      public void UsingCtorReturnsInstanceWithExpectedProperties() {}
    }

    [TestFixture]
    public class WithEmptyStoreAndEmptyUnitOfWork {
      Repository<AggregateRootEntityStub> _sut;
      Model _model;

      [SetUp]
      public void SetUp() {
        EmbeddedEventStore.Instance.Connection.DeleteAllStreams();
        _model = new Model();
        _sut = new RepositoryScenarioBuilder().BuildForRepository();
      }

      [Test]
      public void GetThrows() {
        var exception =
          Assert.Throws<AggregateNotFoundException>(() => _sut.Get(_model.UnknownIdentifier));
        Assert.That(exception.Identifier, Is.EqualTo(_model.UnknownIdentifier));
        Assert.That(exception.Type, Is.EqualTo(typeof(AggregateRootEntityStub)));
      }

      [Test]
      public void GetOptionalReturnsEmpty() {
        var result = _sut.GetOptional(_model.UnknownIdentifier);

        Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void AddAttachesToUnitOfWork() {
        var root = AggregateRootEntityStub.Factory();

        _sut.Add(_model.KnownIdentifier, root);

        Aggregate aggregate;
        var result = _sut.UnitOfWork.TryGet(_model.KnownIdentifier, out aggregate);
        Assert.That(result, Is.True);
        Assert.That(aggregate.Identifier, Is.EqualTo(_model.KnownIdentifier));
        Assert.That(aggregate.Root, Is.SameAs(root));
      }
    }

    [TestFixture]
    public class WithEmptyStoreAndFilledUnitOfWork {
      Repository<AggregateRootEntityStub> _sut;
      AggregateRootEntityStub _root;
      Model _model;

      [SetUp]
      public void SetUp() {
        EmbeddedEventStore.Instance.Connection.DeleteAllStreams();
        _model = new Model();
        _root = AggregateRootEntityStub.Factory();
        _sut = new RepositoryScenarioBuilder().
          ScheduleAttachToUnitOfWork(new Aggregate(_model.KnownIdentifier, 0, _root)).
          BuildForRepository();
      }

      [Test]
      public void GetThrowsForUnknownId() {
        var exception =
          Assert.Throws<AggregateNotFoundException>(() => _sut.Get(_model.UnknownIdentifier));
        Assert.That(exception.Identifier, Is.EqualTo(_model.UnknownIdentifier));
        Assert.That(exception.Type, Is.EqualTo(typeof(AggregateRootEntityStub)));
      }

      [Test]
      public void GetReturnsRootOfKnownId() {
        var result = _sut.Get(_model.KnownIdentifier);

        Assert.That(result, Is.SameAs(_root));
      }

      [Test]
      public void GetOptionalReturnsEmptyForUnknownId() {
        var result = _sut.GetOptional(_model.UnknownIdentifier);

        Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void GetOptionalReturnsRootForKnownId() {
        var result = _sut.GetOptional(_model.KnownIdentifier);

        Assert.That(result, Is.EqualTo(new Optional<AggregateRootEntityStub>(_root)));
      }
    }

    [TestFixture]
    public class WithFilledStore {
      Repository<AggregateRootEntityStub> _sut;
      Model _model;

      [SetUp]
      public void SetUp() {
        EmbeddedEventStore.Instance.Connection.DeleteAllStreams();
        _model = new Model();
        _sut = new RepositoryScenarioBuilder().
          ScheduleAppendToStream(_model.KnownIdentifier, new EventStub(1)).
          BuildForRepository();
      }

      [Test]
      public void GetThrowsForUnknownId() {
        var exception =
          Assert.Throws<AggregateNotFoundException>(() => _sut.Get(_model.UnknownIdentifier));
        Assert.That(exception.Identifier, Is.EqualTo(_model.UnknownIdentifier));
        Assert.That(exception.Type, Is.EqualTo(typeof(AggregateRootEntityStub)));
      }

      [Test]
      public void GetReturnsRootOfKnownId() {
        var result = _sut.Get(_model.KnownIdentifier);

        Assert.That(result.RecordedEvents, Is.EquivalentTo(new[] { new EventStub(1) }));
      }

      [Test]
      public void GetOptionalReturnsEmptyForUnknownId() {
        var result = _sut.GetOptional(_model.UnknownIdentifier);

        Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void GetOptionalReturnsRootForKnownId() {
        var result = _sut.GetOptional(_model.KnownIdentifier);

        Assert.That(result.HasValue, Is.True);
        Assert.That(result.Value.RecordedEvents, Is.EquivalentTo(new[] { new EventStub(1) }));
      }
    }
  }
}
