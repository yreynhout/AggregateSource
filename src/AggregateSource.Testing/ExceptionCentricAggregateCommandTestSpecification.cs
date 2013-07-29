using System;

namespace AggregateSource.Testing
{
    public class ExceptionCentricAggregateCommandTestSpecification
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;
        readonly Action<IAggregateRootEntity> _when;
        readonly Exception _throws;

        public ExceptionCentricAggregateCommandTestSpecification(Func<IAggregateRootEntity> sutFactory, object[] givens,
                                                                 Action<IAggregateRootEntity> when, Exception throws)
        {
            _sutFactory = sutFactory;
            _givens = givens;
            _when = when;
            _throws = throws;
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

        public Exception Throws
        {
            get { return _throws; }
        }
    }
}