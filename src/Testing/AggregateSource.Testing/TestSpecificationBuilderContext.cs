using System;
using System.Collections.Generic;
#if !NET20
using System.Linq;
#endif

namespace AggregateSource.Testing
{
    class TestSpecificationBuilderContext
    {
        readonly Fact[] _givens;
        readonly Fact[] _thens;
        readonly object _when;
        readonly Exception _throws;

        public TestSpecificationBuilderContext()
        {
            _givens = Fact.Empty;
            _thens = Fact.Empty;
            _when = null;
            _throws = null;
        }

        TestSpecificationBuilderContext(Fact[] givens, object when, Fact[] thens,
                                        Exception throws)
        {
            _givens = givens;
            _when = when;
            _thens = thens;
            _throws = throws;
        }

        public TestSpecificationBuilderContext AppendGivens(IEnumerable<Fact> facts)
        {
#if NET20
            var givens = new List<Fact>(_givens);
            givens.AddRange(facts);
            return new TestSpecificationBuilderContext(givens.ToArray(), _when, _thens, _throws);
#else
            return new TestSpecificationBuilderContext(_givens.Concat(facts).ToArray(), _when, _thens, _throws);
#endif
        }

        public TestSpecificationBuilderContext SetWhen(object message)
        {
            return new TestSpecificationBuilderContext(_givens, message, _thens, _throws);
        }

        public TestSpecificationBuilderContext AppendThens(IEnumerable<Fact> facts)
        {
#if NET20
            var thens = new List<Fact>(_thens);
            thens.AddRange(facts);
            return new TestSpecificationBuilderContext(_givens, _when, thens.ToArray(), _throws);
#else
            return new TestSpecificationBuilderContext(_givens, _when, _thens.Concat(facts).ToArray(), _throws);
#endif
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