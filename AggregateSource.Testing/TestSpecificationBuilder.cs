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
      return new TestSpecificationBuilder(_context.AppendGivens(events.Select(@event => new Tuple<Guid, object>(id, @event))));
    }

    public IWhenStateBuilder When(object message) {
      return new TestSpecificationBuilder(_context.SetWhen(message));
    }

    public IThenStateBuilder Then(Guid id, params object[] events) {
      return new TestSpecificationBuilder(_context.AppendThens(events.Select(@event => new Tuple<Guid, object>(id, @event))));
    }

    public IThrowStateBuilder Throws(Exception exception) {
      return new TestSpecificationBuilder(_context.SetThrows(exception));
    }

    public TestSpecification Build() {
      return _context.ToSpecification();
    }
  }
}