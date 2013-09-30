using System;
using System.IO;
using System.Threading.Tasks;
using AggregateSource.EventStore.Framework;
using AggregateSource.EventStore.Framework.Snapshots;
using EventStore.ClientAPI;
using FakeItEasy;
using NUnit.Framework;

namespace AggregateSource.EventStore.Snapshots
{
    namespace AsyncSnapshotableRepositoryTests
    {
        [TestFixture]
        public class Construction
        {
            IAsyncSnapshotReader _reader;
            EventReaderConfiguration _configuration;
            IEventStoreConnection _connection;
            ConcurrentUnitOfWork _unitOfWork;
            Func<SnapshotableAggregateRootEntityStub> _factory;

            [SetUp]
            public void SetUp()
            {
                _connection = EmbeddedEventStore.Connection;
                _reader = AsyncSnapshotReaderFactory.Create();
                _configuration = EventReaderConfigurationFactory.Create();
                _unitOfWork = new ConcurrentUnitOfWork();
                _factory = SnapshotableAggregateRootEntityStub.Factory;
            }

            [Test]
            public void FactoryCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() =>
                                                     new AsyncSnapshotableRepository
                                                         <SnapshotableAggregateRootEntityStub>(null, _unitOfWork,
                                                                                               _connection,
                                                                                               _configuration, _reader));
            }

            [Test]
            public void ConcurrentUnitOfWorkCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() =>
                                                     new AsyncSnapshotableRepository
                                                         <SnapshotableAggregateRootEntityStub>(_factory, null,
                                                                                               _connection,
                                                                                               _configuration, _reader));
            }

            [Test]
            public void EventStoreConnectionCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() =>
                                                     new AsyncSnapshotableRepository
                                                         <SnapshotableAggregateRootEntityStub>(_factory, _unitOfWork,
                                                                                               null, _configuration,
                                                                                               _reader));
            }

            [Test]
            public void EventReaderConfigurationCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() =>
                                                     new AsyncSnapshotableRepository
                                                         <SnapshotableAggregateRootEntityStub>(_factory, _unitOfWork,
                                                                                               _connection, null,
                                                                                               _reader));
            }

            [Test]
            public void SnapshotReaderCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() =>
                                                     new AsyncSnapshotableRepository
                                                         <SnapshotableAggregateRootEntityStub>(_factory, _unitOfWork,
                                                                                               _connection,
                                                                                               _configuration, null));
            }
        }

        [TestFixture]
        public class WithEmptyStoreAndEmptyUnitOfWorkAndNoSnapshot
        {
            AsyncSnapshotableRepository<SnapshotableAggregateRootEntityStub> _sut;
            ConcurrentUnitOfWork _unitOfWork;
            Model _model;
            IStreamNameResolver _resolver;
            IAsyncSnapshotReader _reader;

            [SetUp]
            public void SetUp()
            {
                EmbeddedEventStore.Connection.DeleteAllStreams();
                _model = new Model();
                _unitOfWork = new ConcurrentUnitOfWork();
                _resolver = A.Fake<IStreamNameResolver>();
                _reader = A.Fake<IAsyncSnapshotReader>();
                A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).Returns(_model.KnownIdentifier);
                A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).Returns(_model.UnknownIdentifier);
                A.CallTo(() => _reader.ReadOptionalAsync(_model.KnownIdentifier))
                 .Returns(Task.FromResult(Optional<Snapshot>.Empty));
                A.CallTo(() => _reader.ReadOptionalAsync(_model.UnknownIdentifier))
                 .Returns(Task.FromResult(Optional<Snapshot>.Empty));
                _sut = new AsyncSnapshotableRepository<SnapshotableAggregateRootEntityStub>(
                    SnapshotableAggregateRootEntityStub.Factory,
                    _unitOfWork,
                    EmbeddedEventStore.Connection,
                    EventReaderConfigurationFactory.CreateWithResolver(_resolver),
                    _reader);
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
                Assert.That(actualException.Type, Is.EqualTo(typeof (SnapshotableAggregateRootEntityStub)));
            }

            [Test]
            public void GetAsyncReadsSnapshot()
            {
                Catch.ExceptionOf(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier).Result; });

                A.CallTo(() => _reader.ReadOptionalAsync(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetAsyncResolvesStreamName()
            {
                Catch.ExceptionOf(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier).Result; });

                A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetOptionalAsyncReturnsEmpty()
            {
                var result = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

                Assert.That(result, Is.EqualTo(Optional<SnapshotableAggregateRootEntityStub>.Empty));
            }

            [Test]
            public void GetOptionalAsyncReadsSnapshot()
            {
                var _ = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

                A.CallTo(() => _reader.ReadOptionalAsync(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetOptionalAsyncResolvesStreamName()
            {
                var _ = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

                A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void AddAttachesToUnitOfWork()
            {
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
        public class WithEmptyStoreAndEmptyUnitOfWorkAndSnapshot
        {
            AsyncSnapshotableRepository<SnapshotableAggregateRootEntityStub> _sut;
            ConcurrentUnitOfWork _unitOfWork;
            Model _model;
            IStreamNameResolver _resolver;
            IAsyncSnapshotReader _reader;

            [SetUp]
            public void SetUp()
            {
                EmbeddedEventStore.Connection.DeleteAllStreams();
                _model = new Model();
                _unitOfWork = new ConcurrentUnitOfWork();
                _resolver = A.Fake<IStreamNameResolver>();
                _reader = A.Fake<IAsyncSnapshotReader>();
                A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).Returns(_model.KnownIdentifier);
                A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).Returns(_model.UnknownIdentifier);
                A.CallTo(() => _reader.ReadOptionalAsync(_model.KnownIdentifier))
                 .Returns(Task.FromResult(new Optional<Snapshot>(new Snapshot(100, new object()))));
                A.CallTo(() => _reader.ReadOptionalAsync(_model.UnknownIdentifier))
                 .Returns(Task.FromResult(new Optional<Snapshot>(new Snapshot(100, new object()))));
                _sut = new AsyncSnapshotableRepository<SnapshotableAggregateRootEntityStub>(
                    SnapshotableAggregateRootEntityStub.Factory,
                    _unitOfWork,
                    EmbeddedEventStore.Connection,
                    EventReaderConfigurationFactory.CreateWithResolver(_resolver),
                    _reader);
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
                Assert.That(actualException.Type, Is.EqualTo(typeof (SnapshotableAggregateRootEntityStub)));
            }

            [Test]
            public void GetAsyncReadsSnapshot()
            {
                Catch.ExceptionOf(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier).Result; });

                A.CallTo(() => _reader.ReadOptionalAsync(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetAsyncResolvesStreamName()
            {
                Catch.ExceptionOf(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier).Result; });

                A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetOptionalAsyncReturnsEmpty()
            {
                var result = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

                Assert.That(result, Is.EqualTo(Optional<SnapshotableAggregateRootEntityStub>.Empty));
            }

            [Test]
            public void GetOptionalAsyncReadsSnapshot()
            {
                var _ = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

                A.CallTo(() => _reader.ReadOptionalAsync(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetOptionalAsyncResolvesStreamName()
            {
                var _ = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

                A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void AddAttachesToUnitOfWork()
            {
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
        public class WithEmptyStoreAndFilledUnitOfWorkAndNoSnapshot
        {
            AsyncSnapshotableRepository<SnapshotableAggregateRootEntityStub> _sut;
            ConcurrentUnitOfWork _unitOfWork;
            SnapshotableAggregateRootEntityStub _root;
            Model _model;
            IStreamNameResolver _resolver;
            IAsyncSnapshotReader _reader;

            [SetUp]
            public void SetUp()
            {
                EmbeddedEventStore.Connection.DeleteAllStreams();
                _model = new Model();
                _root = SnapshotableAggregateRootEntityStub.Factory();
                _unitOfWork = new ConcurrentUnitOfWork();
                _unitOfWork.Attach(new Aggregate(_model.KnownIdentifier, 0, _root));
                _resolver = A.Fake<IStreamNameResolver>();
                _reader = A.Fake<IAsyncSnapshotReader>();
                A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).Returns(_model.KnownIdentifier);
                A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).Returns(_model.UnknownIdentifier);
                A.CallTo(() => _reader.ReadOptionalAsync(_model.KnownIdentifier))
                 .Returns(Task.FromResult(Optional<Snapshot>.Empty));
                A.CallTo(() => _reader.ReadOptionalAsync(_model.UnknownIdentifier))
                 .Returns(Task.FromResult(Optional<Snapshot>.Empty));
                _sut = new AsyncSnapshotableRepository<SnapshotableAggregateRootEntityStub>(
                    SnapshotableAggregateRootEntityStub.Factory,
                    _unitOfWork,
                    EmbeddedEventStore.Connection,
                    EventReaderConfigurationFactory.CreateWithResolver(_resolver),
                    _reader);
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
                Assert.That(actualException.Type, Is.EqualTo(typeof (SnapshotableAggregateRootEntityStub)));
            }

            [Test]
            public void GetReadsSnapshotOfUnknownId()
            {
                Catch.ExceptionOf(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier); });

                A.CallTo(() => _reader.ReadOptionalAsync(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetResolvesNameOfUnknownId()
            {
                Catch.ExceptionOf(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier); });

                A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetAsyncReturnsRootOfKnownId()
            {
                var result = _sut.GetAsync(_model.KnownIdentifier).Result;

                Assert.That(result, Is.SameAs(_root));
            }

            [Test]
            public void GetAsyncDoesNotReadSnapshotOfKnownId()
            {
                var _ = _sut.GetAsync(_model.KnownIdentifier).Result;

                A.CallTo(() => _reader.ReadOptionalAsync(_model.KnownIdentifier)).MustNotHaveHappened();
            }

            [Test]
            public void GetAsyncDoesNotResolveNameOfKnownId()
            {
                var _ = _sut.GetAsync(_model.KnownIdentifier).Result;

                A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).MustNotHaveHappened();
            }

            [Test]
            public void GetOptionalAsyncReturnsEmptyForUnknownId()
            {
                var result = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

                Assert.That(result, Is.EqualTo(Optional<SnapshotableAggregateRootEntityStub>.Empty));
            }

            [Test]
            public void GetOptionalAsyncReadsSnapshotOfUnknownId()
            {
                var _ = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

                A.CallTo(() => _reader.ReadOptionalAsync(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetOptionalAsyncResolvesNameOfUnknownId()
            {
                var _ = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

                A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetOptionalAsyncReturnsRootForKnownId()
            {
                var result = _sut.GetOptionalAsync(_model.KnownIdentifier).Result;

                Assert.That(result, Is.EqualTo(new Optional<SnapshotableAggregateRootEntityStub>(_root)));
            }

            [Test]
            public void GetOptionalAsyncDoesNotReadSnapshotOfKnownId()
            {
                var _ = _sut.GetOptionalAsync(_model.KnownIdentifier).Result;

                A.CallTo(() => _reader.ReadOptionalAsync(_model.KnownIdentifier)).MustNotHaveHappened();
            }

            [Test]
            public void GetOptionalAsyncDoesNotResolveNameOfKnownId()
            {
                var _ = _sut.GetOptionalAsync(_model.KnownIdentifier).Result;

                A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).MustNotHaveHappened();
            }
        }

        [TestFixture]
        public class WithEmptyStoreAndFilledUnitOfWorkAndSnapshot
        {
            AsyncSnapshotableRepository<SnapshotableAggregateRootEntityStub> _sut;
            ConcurrentUnitOfWork _unitOfWork;
            SnapshotableAggregateRootEntityStub _root;
            Model _model;
            IStreamNameResolver _resolver;
            IAsyncSnapshotReader _reader;

            [SetUp]
            public void SetUp()
            {
                EmbeddedEventStore.Connection.DeleteAllStreams();
                _model = new Model();
                _root = SnapshotableAggregateRootEntityStub.Factory();
                _unitOfWork = new ConcurrentUnitOfWork();
                _unitOfWork.Attach(new Aggregate(_model.KnownIdentifier, 0, _root));
                _resolver = A.Fake<IStreamNameResolver>();
                _reader = A.Fake<IAsyncSnapshotReader>();
                A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).Returns(_model.KnownIdentifier);
                A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).Returns(_model.UnknownIdentifier);
                A.CallTo(() => _reader.ReadOptionalAsync(_model.KnownIdentifier))
                 .Returns(Task.FromResult(new Optional<Snapshot>(new Snapshot(100, new object()))));
                A.CallTo(() => _reader.ReadOptionalAsync(_model.UnknownIdentifier))
                 .Returns(Task.FromResult(new Optional<Snapshot>(new Snapshot(100, new object()))));
                _sut = new AsyncSnapshotableRepository<SnapshotableAggregateRootEntityStub>(
                    SnapshotableAggregateRootEntityStub.Factory,
                    _unitOfWork,
                    EmbeddedEventStore.Connection,
                    EventReaderConfigurationFactory.CreateWithResolver(_resolver),
                    _reader);
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
                Assert.That(actualException.Type, Is.EqualTo(typeof (SnapshotableAggregateRootEntityStub)));
            }

            [Test]
            public void GetReadsSnapshotOfUnknownId()
            {
                Catch.ExceptionOf(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier); });

                A.CallTo(() => _reader.ReadOptionalAsync(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetResolvesNameOfUnknownId()
            {
                Catch.ExceptionOf(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier); });

                A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetAsyncReturnsRootOfKnownId()
            {
                var result = _sut.GetAsync(_model.KnownIdentifier).Result;

                Assert.That(result, Is.SameAs(_root));
            }

            [Test]
            public void GetAsyncDoesNotReadSnapshotOfKnownId()
            {
                var _ = _sut.GetAsync(_model.KnownIdentifier).Result;

                A.CallTo(() => _reader.ReadOptionalAsync(_model.KnownIdentifier)).MustNotHaveHappened();
            }

            [Test]
            public void GetAsyncDoesNotResolveNameOfKnownId()
            {
                var _ = _sut.GetAsync(_model.KnownIdentifier).Result;

                A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).MustNotHaveHappened();
            }

            [Test]
            public void GetOptionalAsyncReturnsEmptyForUnknownId()
            {
                var result = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

                Assert.That(result, Is.EqualTo(Optional<SnapshotableAggregateRootEntityStub>.Empty));
            }

            [Test]
            public void GetOptionalAsyncReadsSnapshotOfUnknownId()
            {
                var _ = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

                A.CallTo(() => _reader.ReadOptionalAsync(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetOptionalAsyncResolvesNameOfUnknownId()
            {
                var _ = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

                A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetOptionalAsyncReturnsRootForKnownId()
            {
                var result = _sut.GetOptionalAsync(_model.KnownIdentifier).Result;

                Assert.That(result, Is.EqualTo(new Optional<SnapshotableAggregateRootEntityStub>(_root)));
            }

            [Test]
            public void GetOptionalAsyncDoesNotReadSnapshotOfKnownId()
            {
                var _ = _sut.GetOptionalAsync(_model.KnownIdentifier).Result;

                A.CallTo(() => _reader.ReadOptionalAsync(_model.KnownIdentifier)).MustNotHaveHappened();
            }

            [Test]
            public void GetOptionalAsyncDoesNotResolveNameOfKnownId()
            {
                var _ = _sut.GetOptionalAsync(_model.KnownIdentifier).Result;

                A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).MustNotHaveHappened();
            }
        }

        [TestFixture]
        public class WithFilledStore
        {
            AsyncSnapshotableRepository<SnapshotableAggregateRootEntityStub> _sut;
            ConcurrentUnitOfWork _unitOfWork;
            SnapshotableAggregateRootEntityStub _root;
            Model _model;
            IStreamNameResolver _resolver;
            IAsyncSnapshotReader _reader;
            object _state;

            [SetUp]
            public void SetUp()
            {
                _model = new Model();
                using (var stream = new MemoryStream())
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        new EventStub(1).Write(writer);
                    }
                    EmbeddedEventStore.Connection.AppendToStream(
                        _model.KnownIdentifier,
                        ExpectedVersion.NoStream,
                        new EventData(
                            Guid.NewGuid(),
                            typeof (EventStub).AssemblyQualifiedName,
                            false,
                            stream.ToArray(),
                            new byte[0]));
                    EmbeddedEventStore.Connection.AppendToStream(
                        _model.KnownIdentifier,
                        ExpectedVersion.Any,
                        new EventData(
                            Guid.NewGuid(),
                            typeof (EventStub).AssemblyQualifiedName,
                            false,
                            stream.ToArray(),
                            new byte[0]));
                }
                _root = SnapshotableAggregateRootEntityStub.Factory();
                _state = new object();
                _unitOfWork = new ConcurrentUnitOfWork();
                _resolver = A.Fake<IStreamNameResolver>();
                _reader = A.Fake<IAsyncSnapshotReader>();
                A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).Returns(_model.KnownIdentifier);
                A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).Returns(_model.UnknownIdentifier);
                A.CallTo(() => _reader.ReadOptionalAsync(_model.KnownIdentifier))
                 .Returns(Task.FromResult(new Optional<Snapshot>(new Snapshot(1, _state))));
                A.CallTo(() => _reader.ReadOptionalAsync(_model.UnknownIdentifier))
                 .Returns(Task.FromResult(new Optional<Snapshot>(new Snapshot(1, _state))));
                _sut = new AsyncSnapshotableRepository<SnapshotableAggregateRootEntityStub>(
                    () => _root,
                    _unitOfWork,
                    EmbeddedEventStore.Connection,
                    EventReaderConfigurationFactory.CreateWithResolver(_resolver),
                    _reader);
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
                Assert.That(actualException.Type, Is.EqualTo(typeof (SnapshotableAggregateRootEntityStub)));
            }

            [Test]
            public void GetAsyncReadsSnapshotOfUnknownId()
            {
                Catch.ExceptionOf(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier).Result; });

                A.CallTo(() => _reader.ReadOptionalAsync(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetAsyncResolvesNameOfUnknownId()
            {
                Catch.ExceptionOf(() => { var _ = _sut.GetAsync(_model.UnknownIdentifier).Result; });

                A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetReturnsRootOfKnownId()
            {
                var result = _sut.GetAsync(_model.KnownIdentifier).Result;

                Assert.That(result, Is.SameAs(_root));
            }

            [Test]
            public void GetReturnsRootRestoredFromSnapshot()
            {
                var result = _sut.GetAsync(_model.KnownIdentifier).Result;

                Assert.That(result.RecordedSnapshot, Is.SameAs(_state));
            }

            [Test]
            public void GetReadsSnapshotOfKnownId()
            {
                var _ = _sut.GetAsync(_model.KnownIdentifier).Result;

                A.CallTo(() => _reader.ReadOptionalAsync(_model.KnownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetResolvesNameOfKnownId()
            {
                var _ = _sut.GetAsync(_model.KnownIdentifier).Result;

                A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetOptionalAsyncReturnsEmptyForUnknownId()
            {
                var result = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

                Assert.That(result, Is.EqualTo(Optional<SnapshotableAggregateRootEntityStub>.Empty));
            }

            [Test]
            public void GetOptionalAsyncReadsSnapshotOfUnknownId()
            {
                var _ = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

                A.CallTo(() => _reader.ReadOptionalAsync(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetOptionalAsyncResolvesNameOfUnknownId()
            {
                var _ = _sut.GetOptionalAsync(_model.UnknownIdentifier).Result;

                A.CallTo(() => _resolver.Resolve(_model.UnknownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetOptionalAsyncReturnsRootForKnownId()
            {
                var result = _sut.GetOptionalAsync(_model.KnownIdentifier).Result;

                Assert.That(result, Is.EqualTo(new Optional<SnapshotableAggregateRootEntityStub>(_root)));
            }

            [Test]
            public void GetOptionalReturnsRootForKnownIdRestoredFromSnapshot()
            {
                var result = _sut.GetOptionalAsync(_model.KnownIdentifier).Result;

                Assert.That(result.Value.RecordedSnapshot, Is.SameAs(_state));
            }

            [Test]
            public void GetOptionalAsyncReadsSnapshotOfKnownId()
            {
                var _ = _sut.GetOptionalAsync(_model.KnownIdentifier).Result;

                A.CallTo(() => _reader.ReadOptionalAsync(_model.KnownIdentifier)).MustHaveHappened();
            }

            [Test]
            public void GetOptionalAsyncResolvesNameOfKnownId()
            {
                var _ = _sut.GetOptionalAsync(_model.KnownIdentifier).Result;

                A.CallTo(() => _resolver.Resolve(_model.KnownIdentifier)).MustHaveHappened();
            }
        }
    }
}