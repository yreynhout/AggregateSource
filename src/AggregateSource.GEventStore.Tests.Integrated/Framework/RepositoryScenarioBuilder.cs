using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AggregateSource.GEventStore.Framework.Snapshots;
using AggregateSource.GEventStore.Snapshots;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore.Framework
{
    public class RepositoryScenarioBuilder
    {
        readonly List<Action<IEventStoreConnection>> _eventStoreSchedule;
        readonly List<Action<UnitOfWork>> _unitOfWorkSchedule;
        readonly List<Action<ConcurrentUnitOfWork>> _concurrentUnitOfWorkSchedule;
        readonly IEventStoreConnection _connection;
        readonly EventReaderConfiguration _eventReaderConfiguration;
        readonly SnapshotReaderConfiguration _snapshotReaderConfiguration;
        readonly UnitOfWork _unitOfWork;
        readonly ConcurrentUnitOfWork _concurrentUnitOfWork;
        readonly EventReader _eventReader;
        readonly AsyncEventReader _asyncEventReader;
        readonly RepositoryConfiguration _repositoryConfiguration;

        public RepositoryScenarioBuilder()
        {
            _connection = EmbeddedEventStore.Instance.Connection;
            _unitOfWork = new UnitOfWork();
            _concurrentUnitOfWork = new ConcurrentUnitOfWork();
            _eventReaderConfiguration = EventReaderConfigurationFactory.Create();
            _snapshotReaderConfiguration = SnapshotReaderConfigurationFactory.Create();
            _eventReader = EventReaderFactory.Create();
            _asyncEventReader = AsyncEventReaderFactory.Create();
            _eventStoreSchedule = new List<Action<IEventStoreConnection>>();
            _unitOfWorkSchedule = new List<Action<UnitOfWork>>();
            _concurrentUnitOfWorkSchedule = new List<Action<ConcurrentUnitOfWork>>();
            _repositoryConfiguration = RepositoryConfigurationFactory.Create();
        }

        //public ScenarioBuilder UseConnection(EventStoreConnection connection) {
        //  if (connection == null) throw new ArgumentNullException("connection");
        //  _connection = connection;
        //  return this;
        //}

        //public ScenarioBuilder ConfigureReadUsing(EventReaderConfiguration configuration) {
        //  if (configuration == null) throw new ArgumentNullException("configuration");
        //  _eventReaderConfiguration = configuration;
        //  return this;
        //}

        //public ScenarioBuilder ConfigureReadUsing(SnapshotReaderConfiguration configuration) {
        //  if (configuration == null) throw new ArgumentNullException("configuration");
        //  _snapshotReaderConfiguration = configuration;
        //  return this;
        //}

        //public RepositoryScenarioBuilder ScheduleCreateStream(string stream) {
        //  if (stream == null) throw new ArgumentNullException("stream");
        //  _eventStoreSchedule.Add(connection => connection.CreateStream(stream, Guid.NewGuid(), false, new byte[0]));
        //  return this;
        //}

        public RepositoryScenarioBuilder ScheduleAppendToStream(string stream, params object[] events)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (events == null) throw new ArgumentNullException("events");
            _eventStoreSchedule.Add(connection =>
                                    connection.AppendToStream(
                                        stream,
                                        ExpectedVersion.Any,
                                        events.Select(_ =>
                                                      new EventData(
                                                          Guid.NewGuid(),
                                                          _.GetType().AssemblyQualifiedName,
                                                          false,
                                                          ToByteArray(_),
                                                          new byte[0]))));
            return this;
        }


        public RepositoryScenarioBuilder ScheduleAppendToStream(string stream, params Snapshot[] snapshots)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (snapshots == null) throw new ArgumentNullException("snapshots");
            _eventStoreSchedule.Add(connection =>
                                    connection.AppendToStream(
                                        stream,
                                        ExpectedVersion.Any,
                                        snapshots.Select(_ =>
                                                         new EventData(
                                                             Guid.NewGuid(),
                                                             _.State.GetType().AssemblyQualifiedName,
                                                             false,
                                                             ToByteArray(_.State),
                                                             BitConverter.GetBytes(_.Version)))));
            return this;
        }

        public RepositoryScenarioBuilder ScheduleDeleteStream(string stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            _eventStoreSchedule.Add(connection => connection.DeleteStream(stream, ExpectedVersion.Any));
            return this;
        }

        static byte[] ToByteArray(object @object)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    ((IBinarySerializer) @object).Write(writer);
                    writer.Flush();
                    return stream.ToArray();
                }
            }
        }

        public RepositoryScenarioBuilder ScheduleAttachToUnitOfWork(Aggregate aggregate)
        {
            if (aggregate == null) throw new ArgumentNullException("aggregate");
            _unitOfWorkSchedule.Add(uow => uow.Attach(aggregate));
            _concurrentUnitOfWorkSchedule.Add(uow => uow.Attach(aggregate));
            return this;
        }

        public AsyncSnapshotableRepository<SnapshotableAggregateRootEntityStub> BuildForAsyncSnapshotableRepository()
        {
            ExecuteScheduledActions();
            return new AsyncSnapshotableRepository<SnapshotableAggregateRootEntityStub>(
                SnapshotableAggregateRootEntityStub.Factory,
                _concurrentUnitOfWork,
                _connection,
                _eventReaderConfiguration,
                new AsyncSnapshotReader(_connection, _snapshotReaderConfiguration));
        }

        public AsyncRepository<AggregateRootEntityStub> BuildForAsyncRepository()
        {
            ExecuteScheduledActions();
            return new AsyncRepository<AggregateRootEntityStub>(
                AggregateRootEntityStub.Factory,
                _concurrentUnitOfWork,
                _asyncEventReader,
                _repositoryConfiguration);
        }

        public SnapshotableRepository<SnapshotableAggregateRootEntityStub> BuildForSnapshotableRepository()
        {
            ExecuteScheduledActions();
            return new SnapshotableRepository<SnapshotableAggregateRootEntityStub>(
                SnapshotableAggregateRootEntityStub.Factory,
                _unitOfWork,
                _connection,
                _eventReaderConfiguration,
                new SnapshotReader(_connection, _snapshotReaderConfiguration));
        }

        public Repository<AggregateRootEntityStub> BuildForRepository()
        {
            ExecuteScheduledActions();
            return new Repository<AggregateRootEntityStub>(
                AggregateRootEntityStub.Factory,
                _unitOfWork, 
                _eventReader, 
                _repositoryConfiguration);
        }

        void ExecuteScheduledActions()
        {
            foreach (var action in _eventStoreSchedule)
            {
                action(_connection);
            }
            foreach (var action in _unitOfWorkSchedule)
            {
                action(_unitOfWork);
            }
            foreach (var action in _concurrentUnitOfWorkSchedule)
            {
                action(_concurrentUnitOfWork);
            }
        }
    }

    //new ScenarioBuilder().
    //UsingConnection(EventStoreConnection connection).
    //ConfigureReadUsing(EventStoreReadConfiguration configuration).
    //ConfigureReadUsing(SnapshotStoreReadConfiguration configuration).
    //CreateStream(string identifier).
    //AppendToStream(string identifier, object[] @events).
    //AppendToStream(string identifier, Snapshot[] snapshots).
    //DeleteStream(string identifier).

    //BuildForAsyncSnapshotableRepositoryScenario(); {
    //  return new AsyncSnapshotableRepository(SnapshotableAggregateRootEntityStub.Factory(), 
    //}
    //BuildForAsyncRepositoryScenario();
    //BuildForSnapshotableRepositoryScenario();
    //BuildForRepositoryScenario();

    //EventStoreConnection Connection { get; }
    //EventStoreReadConfiguration EventReadConfiguration { get; }
    //SnapshotStoreReadConfiguration SnapshotReadConfiguration { get; }

    //Tuple<object, object[]> (snapshot, events) based comparison
}