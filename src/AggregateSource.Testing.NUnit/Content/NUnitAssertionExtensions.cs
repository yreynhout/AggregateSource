using KellermanSoftware.CompareNetObjects;

namespace AggregateSource.Testing {
  public static class NUnitAssertionExtensions {
    public static void Assert(this IEventCentricAggregateConstructorTestSpecificationBuilder builder) {
      builder.Assert(CreateEventComparer());
    }

    public static void Assert(this IExceptionCentricAggregateConstructorTestSpecificationBuilder builder) {
      builder.Assert(CreateExceptionComparer());
    }

    public static void Assert(this IEventCentricAggregateFactoryTestSpecificationBuilder builder) {
      builder.Assert(CreateEventComparer());
    }

    public static void Assert(this IExceptionCentricAggregateFactoryTestSpecificationBuilder builder) {
      builder.Assert(CreateExceptionComparer());
    }

    public static void Assert(this IEventCentricAggregateCommandTestSpecificationBuilder builder) {
      builder.Assert(CreateEventComparer());
    }

    public static void Assert(this IExceptionCentricAggregateCommandTestSpecificationBuilder builder) {
      builder.Assert(CreateExceptionComparer());
    }

    public static void Assert(this IResultCentricAggregateQueryTestSpecificationBuilder builder) {
      builder.Assert(CreateResultComparer());
    }

    public static void Assert(this IExceptionCentricAggregateQueryTestSpecificationBuilder builder) {
      builder.Assert(CreateExceptionComparer());
    }

    static CompareNetObjectsBasedEventComparer CreateEventComparer() {
      return new CompareNetObjectsBasedEventComparer(new CompareObjects());
    }

    static IExceptionComparer CreateExceptionComparer() {
      var comparer = new CompareObjects();
      comparer.ElementsToIgnore.Add("Source");
      comparer.ElementsToIgnore.Add("StackTrace");
      comparer.ElementsToIgnore.Add("TargetSite");
      return new CompareNetObjectsBasedExceptionComparer(comparer);
    }

    static CompareNetObjectsBasedResultComparer CreateResultComparer() {
      return new CompareNetObjectsBasedResultComparer(new CompareObjects());
    }
  }
}