namespace AggregateSource.GEventStore {
  public static class AggregateExtensions {
    public static StreamName GetName(this Aggregate aggregate) {
      return StreamName.Create(aggregate.Id, aggregate.Root.GetType());
    }
  }
}
