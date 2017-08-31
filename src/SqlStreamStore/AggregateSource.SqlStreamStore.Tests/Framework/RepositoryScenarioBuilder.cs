using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AggregateSource;
using SqlStreamStore;
using SqlStreamStore.Streams;
using StreamStoreStore.Json;

namespace SSS.Framework
{
    public class RepositoryScenarioBuilder
    {
        readonly IStreamStore _eventStore;
        readonly List<Action<IStreamStore>> _eventStoreSchedule;
        readonly List<Action<UnitOfWork>> _unitOfWorkSchedule;
        UnitOfWork _unitOfWork;

        public RepositoryScenarioBuilder()
        {
            _eventStore = new InMemoryStreamStore(() => DateTime.UtcNow);
            _unitOfWork = new UnitOfWork();
            _eventStoreSchedule = new List<Action<IStreamStore>>();
            _unitOfWorkSchedule = new List<Action<UnitOfWork>>();
        }

        public RepositoryScenarioBuilder WithUnitOfWork(UnitOfWork value)
        {
            _unitOfWork = value;
            return this;
        }

        public RepositoryScenarioBuilder ScheduleAppendToStream(string stream, params object[] events)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (events == null) throw new ArgumentNullException("events");
            _eventStoreSchedule.Add(
                store =>
                {
                    var messages = events
                        .Select(o =>
                            new NewStreamMessage(
                                messageId: Guid.NewGuid(),
                                type: o.GetType().AssemblyQualifiedName,
                                jsonData: SimpleJson.SerializeObject(o)))
                        .ToList();

                    store.AppendToStream(new StreamId(stream), ExpectedVersion.Any, messages.ToArray(), CancellationToken.None).GetAwaiter().GetResult();
                });
            return this;
        }

        //public RepositoryScenarioBuilder ScheduleSnapshots(params Snapshot[] snapshots)
        //{
        //    if (snapshots == null) throw new ArgumentNullException("snapshots");
        //    _eventStoreSchedule.Add(
        //        store =>
        //        {
        //            foreach (var snapshot in snapshots)
        //                store.Advanced.AddSnapshot(snapshot);
        //        });
        //    return this;
        //}

        public RepositoryScenarioBuilder ScheduleDeleteStream(string stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            _eventStoreSchedule.Add(store => store.DeleteStream(stream).GetAwaiter().GetResult());
            return this;
        }

        public RepositoryScenarioBuilder ScheduleAttachToUnitOfWork(Aggregate aggregate)
        {
            if (aggregate == null) throw new ArgumentNullException("aggregate");
            _unitOfWorkSchedule.Add(uow => uow.Attach(aggregate));
            return this;
        }

        public Repository<AggregateRootEntityStub> BuildForRepository()
        {
            ExecuteScheduledActions();
            return new Repository<AggregateRootEntityStub>(
                AggregateRootEntityStub.Factory,
                _unitOfWork,
                _eventStore);
        }

        //public SnapshotableRepository<SnapshotableAggregateRootEntityStub> BuildForSnapshotableRepository()
        //{
        //    ExecuteScheduledActions();
        //    return new SnapshotableRepository<SnapshotableAggregateRootEntityStub>(
        //        SnapshotableAggregateRootEntityStub.Factory,
        //        _unitOfWork,
        //        _eventStore);
        //}

        void ExecuteScheduledActions()
        {
            foreach (var action in _eventStoreSchedule)
            {
                action(_eventStore);
            }
            foreach (var action in _unitOfWorkSchedule)
            {
                action(_unitOfWork);
            }
        }
    }
}