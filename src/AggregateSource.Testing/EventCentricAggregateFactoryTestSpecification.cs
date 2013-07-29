using System;

namespace AggregateSource.Testing
{
    public class EventCentricAggregateFactoryTestSpecification
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;
        readonly Func<IAggregateRootEntity, IAggregateRootEntity> _when;
        readonly object[] _thens;

        public EventCentricAggregateFactoryTestSpecification(Func<IAggregateRootEntity> sutFactory, object[] givens,
                                                             Func<IAggregateRootEntity, IAggregateRootEntity> when,
                                                             object[] thens)
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

        public Func<IAggregateRootEntity, IAggregateRootEntity> When
        {
            get { return _when; }
        }

        public object[] Thens
        {
            get { return _thens; }
        }
    }
}