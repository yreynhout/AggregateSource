using System;
using System.Linq;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore {
  public static class EventStoreConnectionExtensions {
    public static void DeleteAllStreams(this EventStoreConnection connection) {
      var slice = connection.ReadAllEventsForward(Position.Start, Int32.MaxValue, false);
      var streams = slice.Events.Select(@event => @event.OriginalStreamId).Distinct();
      foreach (var stream in streams) {
        connection.DeleteStream(stream, ExpectedVersion.Any);
      }
    }
  }
}