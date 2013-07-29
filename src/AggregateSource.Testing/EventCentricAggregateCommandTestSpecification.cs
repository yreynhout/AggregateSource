using System;

namespace AggregateSource.Testing
{
    public class EventCentricAggregateCommandTestSpecification
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;
        readonly Action<IAggregateRootEntity> _when;
        readonly object[] _thens;

        public EventCentricAggregateCommandTestSpecification(Func<IAggregateRootEntity> sutFactory, object[] givens,
                                                             Action<IAggregateRootEntity> when, object[] thens)
        {
            _sutFactory = sutFactory;
            _givens = givens;
            _when = when;
            _thens = thens;
        }

        public Func<IAggregateRootEntity> SutFactory
        {
            get { return _sutFactory; }
        }

        public object[] Givens
        {
            get { return _givens; }
        }

        public Action<IAggregateRootEntity> When
        {
            get { return _when; }
        }

        public object[] Thens
        {
            get { return _thens; }
        }
    }
}