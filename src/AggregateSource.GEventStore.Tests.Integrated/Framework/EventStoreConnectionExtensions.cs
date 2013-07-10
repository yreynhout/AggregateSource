using System;
using System.Linq;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore.Framework {
  public static class EventStoreConnectionExtensions {
    public static void DeleteAllStreams(this IEventStoreConnection connection) {
      var slice = connection.ReadAllEventsForward(Position.Start, Int32.MaxValue, false);
      var streams = slice.
        Events.
        Select(_ => _.OriginalStreamId).
        Distinct();
      foreach (var stream in 
        from _ in streams 
        let streamStatusSlice = connection.ReadStreamEventsForward(_, 0, 1, false) 
        where streamStatusSlice.Status != SliceReadStatus.StreamDeleted && 
              streamStatusSlice.Status != SliceReadStatus.StreamNotFound 
        select _) {
        connection.DeleteStream(stream, ExpectedVersion.Any);
      }
    }
  }
}