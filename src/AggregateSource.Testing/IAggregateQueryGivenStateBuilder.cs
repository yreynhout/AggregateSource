using System;

namespace AggregateSource.Testing
{
    public interface IAggregateQueryGivenStateBuilder<out TAggregateRoot> where TAggregateRoot : IAggregateRootEntity
    {
        IAggregateQueryGivenStateBuilder<TAggregateRoot> Given(params object[] events);
        IAggregateQueryWhenStateBuilder<TResult> When<TResult>(Func<TAggregateRoot, TResult> query);
    }
}