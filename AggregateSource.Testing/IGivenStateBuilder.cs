using System;

namespace AggregateSource.Testing {
  public interface IGivenStateBuilder {
    IGivenStateBuilder Given(Guid id, params object[] events);
    IWhenStateBuilder When(object message);
  }
}