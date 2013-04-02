using System;

namespace AggregateSource.Testing {
  public interface IAggregateCommandWhenStateBuilder {
    IAggregateCommandThenStateBuilder Then(params object[] events);
    IAggregateCommandThrowStateBuilder Throws(Exception exception);
  }
}