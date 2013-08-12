using System;
using System.Linq;

namespace AggregateSource.Testing.CollaborationCentric
{
    class TestSpecificationBuilder : IScenarioGivenStateBuilder, IScenarioGivenNoneStateBuilder, IScenarioWhenStateBuilder, IScenarioThenStateBuilder, IScenarioThenNoneStateBuilder, IScenarioThrowStateBuilder
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

        public IScenarioGivenStateBuilder Given(params Fact[] facts)
        {
            if (facts == null) throw new ArgumentNullException("facts");
            return new TestSpecificationBuilder(_context.AppendGivens(facts));
        }

        public IScenarioGivenStateBuilder Given(string identifier, params object[] events)
        {
            if (identifier == null) throw new ArgumentNullException("identifier");
            if (events == null) throw new ArgumentNullException("events");
            return
                new TestSpecificationBuilder(
                    _context.AppendGivens(events.Select(@event => new Fact(identifier, @event))));
        }

        public IScenarioGivenNoneStateBuilder GivenNone()
        {
            return new TestSpecificationBuilder(_context);
        }

        public IScenarioWhenStateBuilder When(object message)
        {
            if (message == null) throw new ArgumentNullException("message");
            return new TestSpecificationBuilder(_context.SetWhen(message));
        }

        public IScenarioThenStateBuilder Then(params Fact[] facts)
        {
            if (facts == null) throw new ArgumentNullException("facts");
            return new TestSpecificationBuilder(_context.AppendThens(facts));
        }

        public IScenarioThenStateBuilder Then(string identifier, params object[] events)
        {
            if (identifier == null) throw new ArgumentNullException("identifier");
            if (events == null) throw new ArgumentNullException("events");
            return
                new TestSpecificationBuilder(
                    _context.AppendThens(events.Select(@event => new Fact(identifier, @event))));
        }

        public IScenarioThenNoneStateBuilder ThenNone()
        {
            return new TestSpecificationBuilder(_context);
        }

        public IScenarioThrowStateBuilder Throws(Exception exception)
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