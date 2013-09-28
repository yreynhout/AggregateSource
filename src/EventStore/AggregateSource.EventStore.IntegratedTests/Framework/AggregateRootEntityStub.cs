using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AggregateSource.EventStore.Framework
{
    public class AggregateRootEntityStub : AggregateRootEntity
    {
        public static readonly Func<AggregateRootEntityStub> Factory = () => new AggregateRootEntityStub();

        readonly List<object> _recordedEvents;

        public AggregateRootEntityStub()
        {
            _recordedEvents = new List<object>();

            Register<EventStub>(_ => _recordedEvents.Add(_));
        }

        public IList<object> RecordedEvents
        {
            get { return new ReadOnlyCollection<object>(_recordedEvents); }
        }
    }
}