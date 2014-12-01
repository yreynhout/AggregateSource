using System;
using System.Linq;
using EventStore.ClientAPI;

namespace AggregateSource.EventStore.Framework
{
    public static class EventStoreConnectionExtensions
    {
        public static void DeleteAllStreams(this IEventStoreConnection connection)
        {
            var slice = connection.
                ReadAllEventsForwardAsync(
                    Position.Start, Int32.MaxValue, false, EmbeddedEventStore.Credentials);
            slice.Wait();
            var streams = slice.Result.
                Events.
                Select(_ => _.OriginalStreamId).
                Where(StreamNameIsNotReserved).
                Distinct();
            foreach (var stream in streams)
            {
                var streamStatusSlice = connection.ReadStreamEventsForwardAsync(stream, 0, 1, false);
                streamStatusSlice.Wait();
                if (streamStatusSlice.Result.Status != SliceReadStatus.StreamDeleted &&
                    streamStatusSlice.Result.Status != SliceReadStatus.StreamNotFound)
                {
                    connection.DeleteStreamAsync(stream, ExpectedVersion.Any, EmbeddedEventStore.Credentials).Wait();
                }
            }
        }

        static bool StreamNameIsNotReserved(string streamName)
        {
            if (streamName.StartsWith("$$$"))
                return false;
            if (streamName.StartsWith("$$"))
                return false;
            if (streamName.StartsWith("$"))
                return false;
            return true;
        }
    }
}