using System;
using System.Linq;

namespace AggregateSource.Testing.CollaborationCentric
{
    class TestSpecificationBuilder : IGivenStateBuilder, IWhenStateBuilder, IThenStateBuilder, IThrowStateBuilder
    {
        readonly TestSpecificationBuilderContext _context;

        public TestSpecificationBuilder()
        {
            _context = new TestSpecificationBuilderContext();
        }

        TestSpecificationBuilder(TestSpecificationBuilderContext context)
        {
            _context = context;
        }

        public IGivenStateBuilder Given(params Tuple<string, object>[] facts)
        {
            if (facts == null) throw new ArgumentNullException("facts");
            return new TestSpecificationBuilder(_context.AppendGivens(facts));
        }

        public IGivenStateBuilder Given(string identifier, params object[] events)
        {
            if (identifier == null) throw new ArgumentNullException("identifier");
            if (events == null) throw new ArgumentNullException("events");
            return
                new TestSpecificationBuilder(
                    _context.AppendGivens(events.Select(@event => new Tuple<string, object>(identifier, @event))));
        }

        public IWhenStateBuilder When(object message)
        {
            if (message == null) throw new ArgumentNullException("message");
            return new TestSpecificationBuilder(_context.SetWhen(message));
        }

        public IThenStateBuilder Then(params Tuple<string, object>[] facts)
        {
            if (facts == null) throw new ArgumentNullException("facts");
            return new TestSpecificationBuilder(_context.AppendThens(facts));
        }

        public IThenStateBuilder Then(string identifier, params object[] events)
        {
            if (identifier == null) throw new ArgumentNullException("identifier");
            if (events == null) throw new ArgumentNullException("events");
            return
                new TestSpecificationBuilder(
                    _context.AppendThens(events.Select(@event => new Tuple<string, object>(identifier, @event))));
        }

        public IThrowStateBuilder Throws(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException("exception");
            return new TestSpecificationBuilder(_context.SetThrows(exception));
        }

        EventCentricTestSpecification IEventCentricTestSpecificationBuilder.Build()
        {
            return _context.ToEventCentricSpecification();
        }

        ExceptionCentricTestSpecification IExceptionCentricTestSpecificationBuilder.Build()
        {
            return _context.ToExceptionCentricSpecification();
        }
    }
}