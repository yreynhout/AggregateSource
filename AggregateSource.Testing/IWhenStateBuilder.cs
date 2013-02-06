using System;

namespace AggregateSource.Testing {
  public interface IWhenStateBuilder {
    IThenStateBuilder Then(Guid id, params object[] events);
    IThrowStateBuilder Throws(Exception exception);
  }
}