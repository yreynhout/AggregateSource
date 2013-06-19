using System;

namespace AggregateSource.GEventStore {
  public class PassThroughStreamNameResolver : IStreamNameResolver {
    public string Resolve(string identifier) {
      if (identifier == null) throw new ArgumentNullException("identifier");
      return identifier;
    }
  }
}