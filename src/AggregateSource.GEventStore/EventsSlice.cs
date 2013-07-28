using System;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore
{
    public class EventsSlice
    {
        public static readonly EventsSlice NotFound = new EventsSlice(SliceReadStatus.StreamNotFound, new object[0], ExpectedVersion.NoStream);
        public static readonly EventsSlice Deleted = new EventsSlice(SliceReadStatus.StreamDeleted, new object[0], ExpectedVersion.NoStream);

        public readonly SliceReadStatus Status;
        public readonly object[] Events;
        public readonly int LastEventNumber;

        public EventsSlice(SliceReadStatus status, object[] events, int lastEventNumber)
        {
            if (events == null) throw new ArgumentNullException("events");
            Status = status;
            Events = events;
            LastEventNumber = lastEventNumber;
        }
    }
}