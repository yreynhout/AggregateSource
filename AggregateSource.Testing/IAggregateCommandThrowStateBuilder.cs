namespace AggregateSource.Testing {
  public interface IAggregateCommandThrowStateBuilder {
    ExceptionCentricAggregateCommandTestSpecification Build();
  }
}