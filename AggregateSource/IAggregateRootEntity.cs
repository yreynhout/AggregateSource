namespace AggregateSource {
  /// <summary>
  /// Aggregate root entity marker interface.
  /// </summary>
  public interface IAggregateRootEntity : IAggregateInitializer, IAggregateChangeTracker {}
}