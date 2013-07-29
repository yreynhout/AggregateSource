using System;

namespace AggregateSource.Testing
{
    internal class AggregateCommandWhenStateBuilder : IAggregateCommandWhenStateBuilder
    {
        private readonly Func<IAggregateRootEntity> _sutFactory;
        private readonly object[] _givens;
        private readonly Action<IAggregateRootEntity> _when;

        public AggregateCommandWhenStateBuilder(Func<IAggregateRootEntity> sutFactory, object[] givens,
                                                Action<IAggregateRootEntity> when)
        {
            _sutFactory = sutFactory;
            _givens = givens;
            _when = when;
        }

        public IAggregateCommandThenStateBuilder Then(params object[] events)
        {
            if (events == null) throw new ArgumentNullException("events");
            return new AggregateCommandThenStateBuilder(_sutFactory, _givens, _when, events);
        }

        public IAggregateCommandThrowStateBuilder Throws(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException("exception");
            return new AggregateCommandThrowStateBuilder(_sutFactory, _givens, _when, exception);
        }
    }
}