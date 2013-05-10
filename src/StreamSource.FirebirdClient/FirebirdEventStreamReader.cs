using System;
using AggregateSource;

namespace StreamSource.FirebirdClient {
  public class FirebirdEventStreamReader : IEventStreamReader {
    public Optional<EventStream> Read(string streamId) {
      throw new NotImplementedException();
    }
  }
}
