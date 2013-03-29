namespace AggregateSource.Testing {
  public interface IAggregateCommandThenStateBuilder {
    IAggregateCommandThenStateBuilder Then(params object[] events);
    EventCentricAggregateCommandTestSpecification Build();
  }
}