using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AggregateSource.EventStore.Framework;
using AggregateSource.EventStore.Framework.Snapshots;

namespace AggregateSource.EventStore.Snapshots
{
    public class SnapshotableAggregateRootEntityStub : AggregateRootEntity, ISnapshotable
    {
        public static readonly Func<SnapshotableAggregateRootEntityStub> Factory =
            () => new SnapshotableAggregateRootEntityStub();

        readonly List<object> _recordedEvents;

        public SnapshotableAggregateRootEntityStub()
        {
            _recordedEvents = new List<object>();

            Register<EventStub>(_ => _recordedEvents.Add(_));
        }

        public object RecordedSnapshot { get; private set; }

        public IList<object> RecordedEvents
        {
            get { return new ReadOnlyCollection<object>(_recordedEvents); }
        }

        public void RestoreSnapshot(object state)
        {
            RecordedSnapshot = state;
        }

        public object TakeSnapshot()
        {
            return new SnapshotStateStub(new Random().Next());
        }
    }
}