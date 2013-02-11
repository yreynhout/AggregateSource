using System;
using System.Linq;

namespace AggregateSource.Testing {
  internal class TestSpecificationBuilder : IGivenStateBuilder, IWhenStateBuilder, IThenStateBuilder, IThrowStateBuilder {
    readonly TestSpecificationBuilderContext _context;

    public TestSpecificationBuilder() {
      _context = new TestSpecificationBuilderContext();  
    }

    TestSpecificationBuilder(TestSpecificationBuilderContext context) {
      _context = context;
    }

    public IGivenStateBuilder Given(Guid id, params object[] events) {
      if (events == null) throw new ArgumentNullException("events");
      return new TestSpecificationBuilder(_context.AppendGivens(events.Select(@event => new Tuple<Guid, object>(id, @event))));
    }

    public IWhenStateBuilder When(object message) {
      if (message == null) throw new ArgumentNullException("message");
      return new TestSpecificationBuilder(_context.SetWhen(message));
    }

    public IThenStateBuilder Then(Guid id, params object[] events) {
      if (events == null) throw new ArgumentNullException("events");
      return new TestSpecificationBuilder(_context.AppendThens(events.Select(@event => new Tuple<Guid, object>(id, @event))));
    }

    public IThrowStateBuilder Throws(Exception exception) {
      if (exception == null) throw new ArgumentNullException("exception");
      return new TestSpecificationBuilder(_context.SetThrows(exception));
    }

    EventCentricTestSpecification IThenStateBuilder.Build() {
      return _context.ToEventCentricSpecification();
    }

    EventCentricTestSpecification IWhenStateBuilder.Build() {
      return _context.ToEventCentricSpecification();
    }

    ExceptionCentricTestSpecification IThrowStateBuilder.Build() {
      return _context.ToExceptionCentricSpecification();
    }
  }
}