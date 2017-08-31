using System;
using System.Threading.Tasks;
using AggregateSource;
using SqlStreamStore;
using NUnit.Framework;
using SSS.Framework;

namespace SSS
{
    namespace RepositoryTests
    {
        [TestFixture]
        public class Construction
        {
            UnitOfWork _unitOfWork;
            Func<AggregateRootEntityStub> _factory;
            IStreamStore _store;

            [SetUp]
            public void SetUp()
            {
                _store = new InMemoryStreamStore(() => DateTime.UtcNow);
                _unitOfWork = new UnitOfWork();
                _factory = AggregateRootEntityStub.Factory;
            }

            [Test]
            public void FactoryCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(
                    () => new Repository<AggregateRootEntityStub>(null, _unitOfWork, _store));
            }

            [Test]
            public void UnitOfWorkCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(
                    () => new Repository<AggregateRootEntityStub>(_factory, null, _store));
            }

            [Test]
            public void EventStoreCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(
                    () => new Repository<AggregateRootEntityStub>(_factory, _unitOfWork, null));
            }

            [Test]
            public void UsingCtorReturnsInstanceWithExpectedProperties()
            {
                var sut = new Repository<AggregateRootEntityStub>(_factory, _unitOfWork, _store);
                Assert.That(sut.RootFactory, Is.SameAs(_factory));
                Assert.That(sut.UnitOfWork, Is.SameAs(_unitOfWork));
                Assert.That(sut.EventStore, Is.SameAs(_store));
            }
        }

        [TestFixture]
        public class WithEmptyStoreAndEmptyUnitOfWork
        {
            Repository<AggregateRootEntityStub> _sut;
            Model _model;

            [SetUp]
            public void SetUp()
            {
                _model = new Model();
                _sut = new RepositoryScenarioBuilder().BuildForRepository();
            }

            [Test]
            public void GetThrows()
            {
                var exception =
                    Assert.ThrowsAsync<AggregateNotFoundException>(() => _sut.GetAsync(_model.UnknownIdentifier));
                Assert.That(exception.Identifier, Is.EqualTo(_model.UnknownIdentifier));
                Assert.That(exception.ClrType, Is.EqualTo(typeof(AggregateRootEntityStub)));
            }

            [Test]
            public async Task GetOptionalReturnsEmpty()
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

        [TestFixture]
        public class WithEmptyStoreAndFilledUnitOfWork
        {
            Repository<AggregateRootEntityStub> _sut;
            AggregateRootEntityStub _root;
            Model _model;

            [SetUp]
            public void SetUp()
            {
                _model = new Model();
                _root = AggregateRootEntityStub.Factory();
                _sut = new RepositoryScenarioBuilder().
                    ScheduleAttachToUnitOfWork(new Aggregate(_model.KnownIdentifier, 0, _root)).
                    BuildForRepository();
            }

            [Test]
            public void GetThrowsForUnknownId()
            {
                var exception =
                    Assert.ThrowsAsync<AggregateNotFoundException>(async () => await _sut.GetAsync(_model.UnknownIdentifier));
                Assert.That(exception.Identifier, Is.EqualTo(_model.UnknownIdentifier));
                Assert.That(exception.ClrType, Is.EqualTo(typeof(AggregateRootEntityStub)));
            }

            [Test]
            public async Task GetReturnsRootOfKnownId()
            {
                var result = await _sut.GetAsync(_model.KnownIdentifier);

                Assert.That(result, Is.SameAs(_root));
            }

            [Test]
            public async Task GetOptionalReturnsEmptyForUnknownId()
            {
                var result = await _sut.GetOptionalAsync(_model.UnknownIdentifier);

                Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
            }

            [Test]
            public async Task GetOptionalReturnsRootForKnownId()
            {
                var result = await _sut.GetOptionalAsync(_model.KnownIdentifier);

                Assert.That(result, Is.EqualTo(new Optional<AggregateRootEntityStub>(_root)));
            }
        }

        [TestFixture]
        public class WithStreamPresentInStore
        {
            Repository<AggregateRootEntityStub> _sut;
            Model _model;

            [SetUp]
            public void SetUp()
            {
                _model = new Model();
                _sut = new RepositoryScenarioBuilder().
                    ScheduleAppendToStream(_model.KnownIdentifier, new EventStub(1)).
                    BuildForRepository();
            }

            [Test]
            public void GetThrowsForUnknownId()
            {
                var exception =
                    Assert.ThrowsAsync<AggregateNotFoundException>(async () => await _sut.GetAsync(_model.UnknownIdentifier));
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
            public async Task GetOptionalReturnsEmptyForUnknownId()
            {
                var result = await _sut.GetOptionalAsync(_model.UnknownIdentifier);

                Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
            }

            [Test]
            public async Task GetOptionalReturnsRootForKnownId()
            {
                var result = await _sut.GetOptionalAsync(_model.KnownIdentifier);

                Assert.That(result.HasValue, Is.True);
                Assert.That(result.Value.RecordedEvents, Is.EquivalentTo(new[] { new EventStub(1) }));
            }
        }

        [TestFixture]
        public class WithDeletedStreamInStore
        {
            Repository<AggregateRootEntityStub> _sut;
            Model _model;

            [SetUp]
            public void SetUp()
            {
                _model = new Model();
                _sut = new RepositoryScenarioBuilder().
                    ScheduleAppendToStream(_model.KnownIdentifier, new EventStub(1)).
                    ScheduleDeleteStream(_model.KnownIdentifier).
                    BuildForRepository();
            }

            [Test]
            public void GetThrowsForUnknownId()
            {
                var exception =
                    Assert.ThrowsAsync<AggregateNotFoundException>(async () => await _sut.GetAsync(_model.UnknownIdentifier));
                Assert.That(exception.Identifier, Is.EqualTo(_model.UnknownIdentifier));
                Assert.That(exception.ClrType, Is.EqualTo(typeof(AggregateRootEntityStub)));
            }

            [Test]
            public void GetThrowsForKnownDeletedId()
            {
                var exception =
                    Assert.ThrowsAsync<AggregateNotFoundException>(() => _sut.GetAsync(_model.KnownIdentifier));
                Assert.That(exception.Identifier, Is.EqualTo(_model.KnownIdentifier));
                Assert.That(exception.ClrType, Is.EqualTo(typeof(AggregateRootEntityStub)));
            }

            [Test]
            public async Task GetOptionalReturnsEmptyForUnknownId()
            {
                var result = await _sut.GetOptionalAsync(_model.UnknownIdentifier);

                Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
            }

            [Test]
            public async Task GetOptionalReturnsEmptyForKnownDeletedId()
            {
                var result = await _sut.GetOptionalAsync(_model.KnownIdentifier);

                Assert.That(result, Is.EqualTo(Optional<AggregateRootEntityStub>.Empty));
            }
        }
    }
}