using System;
using System.Collections.Generic;
using System.Linq;

namespace AggregateSource.Testing {
  internal class TestSpecificationBuilderContext {
    readonly Tuple<Guid, object>[] _givens;
    readonly Tuple<Guid, object>[] _thens;
    readonly object _when;
    readonly Exception _throws;

    public TestSpecificationBuilderContext() {
      _givens = new Tuple<Guid, object>[0];
      _thens = new Tuple<Guid, object>[0];
      _when = null;
      _throws = null;
    }

    TestSpecificationBuilderContext(Tuple<Guid, object>[] givens, object when, Tuple<Guid, object>[] thens, Exception throws) {
      _givens = givens;
      _when = when;
      _thens = thens;
      _throws = throws;
    }

    public TestSpecificationBuilderContext AppendGivens(IEnumerable<Tuple<Guid, object>> events) {
      return new TestSpecificationBuilderContext(_givens.Concat(events).ToArray(), _when, _thens, _throws);
    }

    public TestSpecificationBuilderContext SetWhen(object message) {
      return new TestSpecificationBuilderContext(_givens, message, _thens, _throws);
    }

    public TestSpecificationBuilderContext AppendThens(IEnumerable<Tuple<Guid, object>> events) {
      return new TestSpecificationBuilderContext(_givens, _when, _thens.Concat(events).ToArray(), _throws);
    }

    public TestSpecificationBuilderContext SetThrows(Exception exception) {
      return new TestSpecificationBuilderContext(_givens, _when, _thens, exception);
    }

    public TestSpecification ToSpecification() {
      return new TestSpecification(_givens, _when, _thens, _throws);
    }
  }
}