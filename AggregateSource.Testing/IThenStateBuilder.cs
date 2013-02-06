using System;

namespace AggregateSource.Testing {
  public interface IThenStateBuilder {
    IThenStateBuilder Then(Guid id, params object[] events);
    TestSpecification Build();
  }
}