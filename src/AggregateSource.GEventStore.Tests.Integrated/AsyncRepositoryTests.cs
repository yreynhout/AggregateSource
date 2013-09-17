using System;
using AggregateSource.GEventStore.Framework;
using NUnit.Framework;

namespace AggregateSource.GEventStore
{
    namespace AsyncRepositoryTests
    {
        [TestFixture]
        public class Construction
        {
            [Test]
            public void FactoryCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(
                    () => new AsyncRepository<AggregateRootEntityStub>(
                        null,
                        new ConcurrentUnitOfWork(),
                        EmbeddedEventStore.Connection,
                        EventReaderConfigurationFactory.Create()));
            }

            [Test]
            public void ConcurrentUnitOfWorkCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() =>
                                                     new AsyncRepository<AggregateRootEntityStub>(
                                                         AggregateRootEntityStub.Factory, null,
                                                         EmbeddedEventStore.Connection,
                                                         EventReaderConfigurationFactory.Create()));
            }

            [Test]
            public void EventStoreConnectionCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() =>
                                                     new AsyncRepository<AggregateRootEntityStub>(
                                                         AggregateRootEntityStub.Factory, new ConcurrentUnitOfWork(),
                                                         null, EventReaderConfigurationFactory.Create()));
            }

            [Test]
            public void EventReaderConfigurationCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() =>
                                                     new AsyncRepository<AggregateRootEntityStub>(
                                                         AggregateRootEntityStub.Factory, new ConcurrentUnitOfWork(),
                                                         EmbeddedEventStore.Connection, null));
            }

            [Ignore("TODO after merge - requires setup")]
            public void UsingCtorReturnsInstanceWithExpectedProperties() {}
        }

        [TestFixture]
        public class WithEmptyStoreAndEmptyUnitOfWork
        {
            AsyncRepository<AggregateRootEntityStub> _sut;
            Model _model;

            [SetUp]
            public void SetUp()
            {
                EmbeddedEventStore.Connection.DeleteAllStreams();
                _model = new Model();
                _sut = new RepositoryScenarioBuilder().BuildForAsyncRepository();
            }

            [Test]
            public void GetAsyncThrows()
            {
                var exception =
                    Assert.Throws<AggregateException>(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier).Result; });
                Assert.That(exception.InnerExceptions, Has.Count.EqualTo(1));
                Assert.That(exception.InnerExceptions[0], Is.InstanceOf<AggregateNotFoundException>());
                var actualException = (AggregateNotFoundException) exception.InnerExceptions[0];
                Assert.That(actualException.Identifier, Is.EqualTo(_model.UnknownIdentifier));
                Assert.That(actualException.Type, Is.EqualTo(typeof (AggregateRootEntityStub)));
            }

            [Test]
            public void GetOptionalAsyncReturnsEmpty()
            {
                var result = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

                Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
            }

            [Test]
            public void AddAttachesToUnitOfWork()
            {
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
        public class WithEmptyStoreAndFilledUnitOfWork
        {
            AsyncRepository<AggregateRootEntityStub> _sut;
            AggregateRootEntityStub _root;
            Model _model;

            [SetUp]
            public void SetUp()
            {
                EmbeddedEventStore.Connection.DeleteAllStreams();
                _model = new Model();
                _root = AggregateRootEntityStub.Factory();
                _sut = new RepositoryScenarioBuilder().
                    ScheduleAttachToUnitOfWork(new Aggregate(_model.KnownIdentifier, 0, _root)).
                    BuildForAsyncRepository();
            }

            [Test]
            public void GetAsyncThrowsForUnknownId()
            {
                var exception =
                    Assert.Throws<AggregateException>(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier).Result; });
                Assert.That(exception.InnerExceptions, Has.Count.EqualTo(1));
                Assert.That(exception.InnerExceptions[0], Is.InstanceOf<AggregateNotFoundException>());
                var actualException = (AggregateNotFoundException) exception.InnerExceptions[0];
                Assert.That(actualException.Identifier, Is.EqualTo(_model.UnknownIdentifier));
                Assert.That(actualException.Type, Is.EqualTo(typeof (AggregateRootEntityStub)));
            }

            [Test]
            public void GetAsyncReturnsRootOfKnownId()
            {
                var result = _sut.GetAsync(_model.KnownIdentifier).Result;

                Assert.That(result, Is.SameAs(_root));
            }

            [Test]
            public void GetOptionalAsyncReturnsEmptyForUnknownId()
            {
                var result = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

                Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
            }

            [Test]
            public void GetOptionalAsyncReturnsRootForKnownId()
            {
                var result = _sut.GetOptionalAsync(_model.KnownIdentifier).Result;

                Assert.That(result, Is.EqualTo(new Optional<AggregateRootEntityStub>(_root)));
            }
        }

        [TestFixture]
        public class WithStreamPresentInStore
        {
            AsyncRepository<AggregateRootEntityStub> _sut;
            Model _model;

            [SetUp]
            public void SetUp()
            {
                EmbeddedEventStore.Connection.DeleteAllStreams();
                _model = new Model();
                _sut = new RepositoryScenarioBuilder().
                    ScheduleAppendToStream(_model.KnownIdentifier, new EventStub(1)).
                    BuildForAsyncRepository();
            }

            [Test]
            public void GetAsyncThrowsForUnknownId()
            {
                var exception =
                    Assert.Throws<AggregateException>(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier).Result; });
                Assert.That(exception.InnerExceptions, Has.Count.EqualTo(1));
                Assert.That(exception.InnerExceptions[0], Is.InstanceOf<AggregateNotFoundException>());
                var actualException = (AggregateNotFoundException) exception.InnerExceptions[0];
                Assert.That(actualException.Identifier, Is.EqualTo(_model.UnknownIdentifier));
                Assert.That(actualException.Type, Is.EqualTo(typeof (AggregateRootEntityStub)));
            }

            [Test]
            public void GetReturnsRootOfKnownId()
            {
                var result = _sut.GetAsync(_model.KnownIdentifier).Result;

                Assert.That(result.RecordedEvents, Is.EquivalentTo(new[] {new EventStub(1)}));
            }

            [Test]
            public void GetOptionalAsyncReturnsEmptyForUnknownId()
            {
                var result = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

                Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
            }

            [Test]
            public void GetOptionalAsyncReturnsRootForKnownId()
            {
                var result = _sut.GetOptionalAsync(_model.KnownIdentifier).Result;

                Assert.That(result.HasValue, Is.True);
                Assert.That(result.Value.RecordedEvents, Is.EquivalentTo(new[] {new EventStub(1)}));
            }
        }

        [TestFixture]
        public class WithDeletedStreamInStore
        {
            AsyncRepository<AggregateRootEntityStub> _sut;
            Model _model;

            [SetUp]
            public void SetUp()
            {
                EmbeddedEventStore.Connection.DeleteAllStreams();
                _model = new Model();
                _sut = new RepositoryScenarioBuilder().
                    ScheduleAppendToStream(_model.KnownIdentifier, new EventStub(1)).
                    ScheduleDeleteStream(_model.KnownIdentifier).
                    BuildForAsyncRepository();
            }

            [Test]
            public void GetAsyncThrowsForUnknownId()
            {
                var exception =
                    Assert.Throws<AggregateException>(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier).Result; });
                Assert.That(exception.InnerExceptions, Has.Count.EqualTo(1));
                Assert.That(exception.InnerExceptions[0], Is.InstanceOf<AggregateNotFoundException>());
                var actualException = (AggregateNotFoundException) exception.InnerExceptions[0];
                Assert.That(actualException.Identifier, Is.EqualTo(_model.UnknownIdentifier));
                Assert.That(actualException.Type, Is.EqualTo(typeof (AggregateRootEntityStub)));
            }

            [Test]
            public void GetAsyncThrowsForKnownId()
            {
                var exception =
                    Assert.Throws<AggregateException>(() => { var _ = _sut.GetAsync(_model.KnownIdentifier).Result; });
                Assert.That(exception.InnerExceptions, Has.Count.EqualTo(1));
                Assert.That(exception.InnerExceptions[0], Is.InstanceOf<AggregateNotFoundException>());
                var actualException = (AggregateNotFoundException) exception.InnerExceptions[0];
                Assert.That(actualException.Identifier, Is.EqualTo(_model.KnownIdentifier));
                Assert.That(actualException.Type, Is.EqualTo(typeof (AggregateRootEntityStub)));
            }

            [Test]
            public void GetOptionalAsyncReturnsEmptyForUnknownId()
            {
                var result = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

                Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
            }

            [Test]
            public void GetOptionalAsyncReturnsEmptyForKnownId()
            {
                var result = _sut.GetOptionalAsync(_model.KnownIdentifier).Result;

                Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
            }
        }
    }
}