using System;

namespace AggregateSource.Testing
{
    public interface IAggregateQueryWhenStateBuilder<TResult>
    {
        IAggregateQueryThenStateBuilder Then(TResult result);
        IAggregateQueryThrowStateBuilder Throws(Exception exception);
    }
}