using System;
using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;

namespace AggregateSource.GEventStore {
  public static class EventStoreConnectionExtensions {
    private static readonly HashSet<String> DeletedStreams = new HashSet<string>();

    public static void DeleteAllStreams(this EventStoreConnection connection) {
      var slice = connection.ReadAllEventsForward(Position.Start, Int32.MaxValue, false);
      var streams = slice.
        Events.
        Where(_ => !DeletedStreams.Contains(_.OriginalStreamId)).
        Select(_ => _.OriginalStreamId).Distinct();
      foreach (var stream in streams) {
        connection.DeleteStream(stream, ExpectedVersion.Any);
        DeletedStreams.Add(stream);
      }
    }
  }
}