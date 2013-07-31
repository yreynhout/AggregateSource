using System;
using System.Collections.Generic;
using System.Linq;

namespace AggregateSource.Testing.CollaborationCentric
{
    class TestSpecificationBuilderContext
    {
        readonly Tuple<string, object>[] _givens;
        readonly Tuple<string, object>[] _thens;
        readonly object _when;
        readonly Exception _throws;

        public TestSpecificationBuilderContext()
        {
            _givens = new Tuple<string, object>[0];
            _thens = new Tuple<string, object>[0];
            _when = null;
            _throws = null;
        }

        TestSpecificationBuilderContext(Tuple<string, object>[] givens, object when, Tuple<string, object>[] thens,
                                        Exception throws)
        {
            _givens = givens;
            _when = when;
            _thens = thens;
            _throws = throws;
        }

        public TestSpecificationBuilderContext AppendGivens(IEnumerable<Tuple<string, object>> events)
        {
            return new TestSpecificationBuilderContext(_givens.Concat(events).ToArray(), _when, _thens, _throws);
        }

        public TestSpecificationBuilderContext SetWhen(object message)
        {
            return new TestSpecificationBuilderContext(_givens, message, _thens, _throws);
        }

        public TestSpecificationBuilderContext AppendThens(IEnumerable<Tuple<string, object>> events)
        {
            return new TestSpecificationBuilderContext(_givens, _when, _thens.Concat(events).ToArray(), _throws);
        }

        public TestSpecificationBuilderContext SetThrows(Exception exception)
        {
            return new TestSpecificationBuilderContext(_givens, _when, _thens, exception);
        }

        public EventCentricTestSpecification ToEventCentricSpecification()
        {
            return new EventCentricTestSpecification(_givens, _when, _thens);
        }

        public ExceptionCentricTestSpecification ToExceptionCentricSpecification()
        {
            return new ExceptionCentricTestSpecification(_givens, _when, _throws);
        }
    }
}