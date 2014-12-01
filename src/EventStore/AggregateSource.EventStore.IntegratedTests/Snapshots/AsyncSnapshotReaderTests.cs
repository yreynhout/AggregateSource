#if !NET40
using System;
using System.IO;
using AggregateSource.EventStore.Framework;
using AggregateSource.EventStore.Framework.Snapshots;
using EventStore.ClientAPI;
using NUnit.Framework;

namespace AggregateSource.EventStore.Snapshots
{
    namespace AsyncSnapshotReaderTests
    {
        [TestFixture]
        public class WithAnyInstance
        {
            SnapshotReaderConfiguration _configuration;
            IEventStoreConnection _connection;

            [SetUp]
            public void SetUp()
            {
                _configuration = SnapshotReaderConfigurationFactory.Create();
                _connection = EmbeddedEventStore.Connection;
            }

            [Test]
            public void ConnectionCannotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => new AsyncSnapshotReader(null, _configuration));
            }

            [Test]
            public void ConfigurationCannotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => new AsyncSnapshotReader(_connection, null));
            }

            [Test]
            public void IsAsyncSnapshotReader()
            {
                Assert.That(AsyncSnapshotReaderFactory.Create(), Is.InstanceOf<IAsyncSnapshotReader>());
            }

            [Test]
            public void ConfigurationReturnsExpectedValue()
            {
                var configuration = SnapshotReaderConfigurationFactory.Create();
                Assert.That(AsyncSnapshotReaderFactory.CreateWithConfiguration(configuration).Configuration,
                            Is.SameAs(configuration));
            }

            [Test]
            public void ConnectionReturnsExpectedValue()
            {
                var connection = EmbeddedEventStore.Connection;
                Assert.That(AsyncSnapshotReaderFactory.CreateWithConnection(connection).Connection,
                            Is.SameAs(connection));
            }

            [Test]
            public void ReadIdentifierCannotBeNull()
            {
                var sut = AsyncSnapshotReaderFactory.Create();
                var exception = Assert.Throws<AggregateException>(() => { var _ = sut.ReadOptionalAsync(null).Result; });
                Assert.That(exception.InnerExceptions, Has.Count.EqualTo(1));
                Assert.That(exception.InnerExceptions[0], Is.InstanceOf<ArgumentNullException>());
            }
        }

        [TestFixture]
        public class WithSnapshotStreamFoundInStore
        {
            Model _model;
            AsyncSnapshotReader _sut;

            [SetUp]
            public void SetUp()
            {
                _model = new Model();
                _sut = AsyncSnapshotReaderFactory.Create();
                CreateSnapshotStreamWithOneSnapshot(_sut.Configuration.StreamNameResolver.Resolve(_model.KnownIdentifier));
            }

            static void CreateSnapshotStreamWithOneSnapshot(string snapshotStreamName)
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        new SnapshotStateStub(1).Write(writer);
                    }
                    EmbeddedEventStore.Connection.AppendToStreamAsync(
                        snapshotStreamName,
                        ExpectedVersion.NoStream,
                        new EventData(
                            Guid.NewGuid(),
                            typeof (SnapshotStateStub).AssemblyQualifiedName,
                            false,
                            stream.ToArray(),
                            BitConverter.GetBytes(100))).Wait();
                }
            }

            [Test]
            public void GetReturnsSnapshotOfKnownId()
            {
                var result = _sut.ReadOptionalAsync(_model.KnownIdentifier).Result;

                Assert.That(result, Is.EqualTo(new Optional<Snapshot>(new Snapshot(100, new SnapshotStateStub(1)))));
            }

            [Test]
            public void GetReturnsEmptyForUnknownId()
            {
                var result = _sut.ReadOptionalAsync(_model.UnknownIdentifier).Result;

                Assert.That(result, Is.EqualTo(Optional<Snapshot>.Empty));
            }
        }

        [TestFixture]
        public class WithSnapshotStreamNotFoundInStore
        {
            Model _model;
            AsyncSnapshotReader _sut;

            [SetUp]
            public void SetUp()
            {
                _model = new Model();
                _sut = AsyncSnapshotReaderFactory.Create();
            }

            [Test]
            public void GetReturnsEmptyForKnownId()
            {
                var result = _sut.ReadOptionalAsync(_model.KnownIdentifier).Result;

                Assert.That(result, Is.EqualTo(Optional<Snapshot>.Empty));
            }
        }

        [TestFixture]
        public class WithEmptySnapshotStreamInStore
        {
            Model _model;
            AsyncSnapshotReader _sut;

            [SetUp]
            public void SetUp()
            {
                _model = new Model();
                _sut = AsyncSnapshotReaderFactory.Create();
                CreateEmptySnapshotStream(_sut.Configuration.StreamNameResolver.Resolve(_model.KnownIdentifier));
            }

            static void CreateEmptySnapshotStream(string snapshotStreamName)
            {
                EmbeddedEventStore.Connection.AppendToStreamAsync(
                    snapshotStreamName,
                    ExpectedVersion.Any,
                    new EventData[0]).
                    Wait();
            }

            [Test]
            public void GetReturnsEmptyForKnownId()
            {
                var result = _sut.ReadOptionalAsync(_model.KnownIdentifier).Result;

                Assert.That(result, Is.EqualTo(Optional<Snapshot>.Empty));
            }

            [Test]
            public void GetReturnsEmptyForUnknownId()
            {
                var result = _sut.ReadOptionalAsync(_model.UnknownIdentifier).Result;

                Assert.That(result, Is.EqualTo(Optional<Snapshot>.Empty));
            }
        }

        [TestFixture]
        public class WithDeletedSnapshotStreamInStore
        {
            Model _model;
            AsyncSnapshotReader _sut;

            [SetUp]
            public void SetUp()
            {
                _model = new Model();
                _sut = AsyncSnapshotReaderFactory.Create();
                CreateDeletedSnapshotStream(_sut.Configuration.StreamNameResolver.Resolve(_model.KnownIdentifier));
            }

            static void CreateDeletedSnapshotStream(string snapshotStreamName)
            {
                EmbeddedEventStore.Connection.AppendToStreamAsync(
                    snapshotStreamName,
                    ExpectedVersion.Any,
                    new EventData[0]).Wait();
                EmbeddedEventStore.Connection.DeleteStreamAsync(
                    snapshotStreamName,
                    ExpectedVersion.EmptyStream).Wait();
            }

            [Test]
            public void GetReturnsSnapshotOfKnownId()
            {
                var result = _sut.ReadOptionalAsync(_model.KnownIdentifier).Result;

                Assert.That(result, Is.EqualTo(Optional<Snapshot>.Empty));
            }

            [Test]
            public void GetReturnsEmptyForUnknownId()
            {
                var result = _sut.ReadOptionalAsync(_model.UnknownIdentifier).Result;

                Assert.That(result, Is.EqualTo(Optional<Snapshot>.Empty));
            }
        }
    }
}
#endif