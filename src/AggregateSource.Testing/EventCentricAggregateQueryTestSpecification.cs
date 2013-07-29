using System;

namespace AggregateSource.Testing
{
    public class EventCentricAggregateQueryTestSpecification
    {
        private readonly Func<IAggregateRootEntity> _sutFactory;
        private readonly object[] _givens;
        private readonly Func<IAggregateRootEntity, object> _when;
        private readonly object _then;

        public EventCentricAggregateQueryTestSpecification(Func<IAggregateRootEntity> sutFactory, object[] givens,
                                                           Func<IAggregateRootEntity, object> when, object then)
        {
            _sutFactory = sutFactory;
            _givens = givens;
            _when = when;
            _then = then;
        }

        public Func<IAggregateRootEntity> SutFactory
        {
            get { return _sutFactory; }
        }

        public object[] Givens
        {
            get { return _givens; }
        }

        public Func<IAggregateRootEntity, object> When
        {
            get { return _when; }
        }

        public object Then
        {
            get { return _then; }
        }
    }
}