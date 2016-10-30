using System.Threading.Tasks;
#if !NET40
using System;
using AggregateSource.EventStore.Framework;
using NUnit.Framework;

namespace AggregateSource.EventStore
{
    namespace AsyncRepositoryTests
    {
        // ReSharper disable UnusedVariable
        [TestFixture, SingleThreaded]
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
            public void UsingCtorReturnsInstanceWithExpectedProperties() { }
        }

        [TestFixture, SingleThreaded]
        public class WithEmptyStoreAndEmptyUnitOfWork
        {
            AsyncRepository<AggregateRootEntityStub> _sut;
            Model _model;

            [SetUp]
            public async Task SetUp()
            {
                await EmbeddedEventStore.Connection.DeleteAllStreamsAsync();
                _model = new Model();
                _sut = new RepositoryScenarioBuilder().BuildForAsyncRepository();
            }

            [Test]
            public void GetAsyncThrows()
            {
                var exception =
                    Assert.ThrowsAsync<AggregateNotFoundException>(async () => { var _ = await _sut.GetAsync(_model.UnknownIdentifier); });
                Assert.That(exception.Identifier, Is.EqualTo(_model.UnknownIdentifier));
                Assert.That(exception.ClrType, Is.EqualTo(typeof(AggregateRootEntityStub)));
            }

            [Test]
            public async Task GetOptionalAsyncReturnsEmpty()
            {
                var result = await _sut.GetOptionalAsync(_model.UnknownIdentifier);

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

        [TestFixture, SingleThreaded]
        public class WithEmptyStoreAndFilledUnitOfWork
        {
            AsyncRepository<AggregateRootEntityStub> _sut;
            AggregateRootEntityStub _root;
            Model _model;

            [SetUp]
            public async Task SetUp()
            {
                await EmbeddedEventStore.Connection.DeleteAllStreamsAsync();
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
                    Assert.ThrowsAsync<AggregateNotFoundException>(async () => { var _ = await _sut.GetAsync(_model.UnknownIdentifier); });
                Assert.That(exception.Identifier, Is.EqualTo(_model.UnknownIdentifier));
                Assert.That(exception.ClrType, Is.EqualTo(typeof(AggregateRootEntityStub)));
            }

            [Test]
            public async Task GetAsyncReturnsRootOfKnownId()
            {
                var result = await _sut.GetAsync(_model.KnownIdentifier);

                Assert.That(result, Is.SameAs(_root));
            }

            [Test]
            public async Task GetOptionalAsyncReturnsEmptyForUnknownId()
            {
                var result = await _sut.GetOptionalAsync(_model.UnknownIdentifier);

                Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
            }

            [Test]
            public async Task GetOptionalAsyncReturnsRootForKnownId()
            {
                var result = await _sut.GetOptionalAsync(_model.KnownIdentifier);

                Assert.That(result, Is.EqualTo(new Optional<AggregateRootEntityStub>(_root)));
            }
        }

        [TestFixture, SingleThreaded]
        public class WithStreamPresentInStore
        {
            AsyncRepository<AggregateRootEntityStub> _sut;
            Model _model;

            [SetUp]
            public async Task SetUp()
            {
                await EmbeddedEventStore.Connection.DeleteAllStreamsAsync();
                _model = new Model();
                _sut = new RepositoryScenarioBuilder().
                    ScheduleAppendToStream(_model.KnownIdentifier, new EventStub(1)).
                    BuildForAsyncRepository();
            }

            [Test]
            public void GetAsyncThrowsForUnknownId()
            {
                var exception =
                    Assert.ThrowsAsync<AggregateNotFoundException>(async () => { var _ = await _sut.GetAsync(_model.UnknownIdentifier); });
                Assert.That(exception.Identifier, Is.EqualTo(_model.UnknownIdentifier));
                Assert.That(exception.ClrType, Is.EqualTo(typeof(AggregateRootEntityStub)));
            }

            [Test]
            public async Task GetReturnsRootOfKnownId()
            {
                var result = await _sut.GetAsync(_model.KnownIdentifier);

                Assert.That(result.RecordedEvents, Is.EquivalentTo(new[] { new EventStub(1) }));
            }

            [Test]
            public async Task GetOptionalAsyncReturnsEmptyForUnknownId()
            {
                var result = await _sut.GetOptionalAsync(_model.UnknownIdentifier);

                Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
            }

            [Test]
            public async Task GetOptionalAsyncReturnsRootForKnownId()
            {
                var result = await _sut.GetOptionalAsync(_model.KnownIdentifier);

                Assert.That(result.HasValue, Is.True);
                Assert.That(result.Value.RecordedEvents, Is.EquivalentTo(new[] { new EventStub(1) }));
            }
        }

        [TestFixture, SingleThreaded]
        public class WithDeletedStreamInStore
        {
            AsyncRepository<AggregateRootEntityStub> _sut;
            Model _model;

            [SetUp]
            public async Task SetUp()
            {
                await EmbeddedEventStore.Connection.DeleteAllStreamsAsync();
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
                    Assert.ThrowsAsync<AggregateNotFoundException>(async () => { var _ = await _sut.GetAsync(_model.UnknownIdentifier); });
                Assert.That(exception.Identifier, Is.EqualTo(_model.UnknownIdentifier));
                Assert.That(exception.ClrType, Is.EqualTo(typeof(AggregateRootEntityStub)));
            }

            [Test]
            public void GetAsyncThrowsForKnownId()
            {
                var exception =
                    Assert.ThrowsAsync<AggregateNotFoundException>(async () => { var _ = await _sut.GetAsync(_model.KnownIdentifier); });
                Assert.That(exception.Identifier, Is.EqualTo(_model.KnownIdentifier));
                Assert.That(exception.ClrType, Is.EqualTo(typeof(AggregateRootEntityStub)));
            }

            [Test]
            public async Task GetOptionalAsyncReturnsEmptyForUnknownId()
            {
                var result = await _sut.GetOptionalAsync(_model.UnknownIdentifier);

                Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
            }

            [Test]
            public async Task GetOptionalAsyncReturnsEmptyForKnownId()
            {
                var result = await _sut.GetOptionalAsync(_model.KnownIdentifier);

                Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
            }
        }
        // ReSharper restore UnusedVariable
    }
}
#endif