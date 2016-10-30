using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace AggregateSource.EventStore.Framework
{
    public static class EventStoreConnectionExtensions
    {
        public static Task DeleteAllStreamsAsync(this IEventStoreConnection connection)
        {
            return Task.FromResult<object>(null);
            //AllEventsSlice slice;
            //var position = Position.Start;
            //do
            //{
            //    slice = await connection.
            //        ReadAllEventsForwardAsync(
            //            position, 4096, false, EmbeddedEventStore.Credentials);
            //    var streams = slice.
            //        Events.
            //        Select(_ => _.OriginalStreamId).
            //        Where(StreamNameIsNotReserved).
            //        Distinct();
            //    //foreach (var stream in streams)
            //    //{ 
            //    //    var streamStatusSlice = await connection.ReadStreamEventsForwardAsync(stream, 0, 1, false, EmbeddedEventStore.Credentials);
            //    //    if (streamStatusSlice.Status != SliceReadStatus.StreamDeleted &&
            //    //        streamStatusSlice.Status != SliceReadStatus.StreamNotFound)
            //    //    {
            //    //        await connection.DeleteStreamAsync(stream, ExpectedVersion.Any, EmbeddedEventStore.Credentials);
            //    //    }
            //    //}
            //    position = slice.NextPosition;
            //} while (!slice.IsEndOfStream);
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