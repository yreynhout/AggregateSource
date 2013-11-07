using System;
#if NET20
using System.Collections.Generic;
#endif
#if !NET20
using System.Linq;
#endif

namespace AggregateSource.Testing.Command
{
    class AggregateCommandGivenStateBuilder<TAggregateRoot> : IAggregateCommandGivenStateBuilder<TAggregateRoot>
        where TAggregateRoot : IAggregateRootEntity
    {
        readonly Func<IAggregateRootEntity> _sutFactory;
        readonly object[] _givens;

        public AggregateCommandGivenStateBuilder(Func<IAggregateRootEntity> sutFactory, object[] givens)
        {
            _sutFactory = sutFactory;
            _givens = givens;
        }

        public IAggregateCommandGivenStateBuilder<TAggregateRoot> Given(params object[] events)
        {
            if (events == null) throw new ArgumentNullException("events");
#if NET20
            var givens = new List<object>();
            givens.AddRange(_givens);
            givens.AddRange(events);
            return new AggregateCommandGivenStateBuilder<TAggregateRoot>(_sutFactory, givens.ToArray());
#else
            return new AggregateCommandGivenStateBuilder<TAggregateRoot>(_sutFactory, _givens.Concat(events).ToArray());
#endif
        }

        public IAggregateCommandWhenStateBuilder When(Action<TAggregateRoot> command)
        {
            if (command == null) throw new ArgumentNullException("command");
            return new AggregateCommandWhenStateBuilder(_sutFactory, _givens, root => command((TAggregateRoot) root));
        }
    }
}