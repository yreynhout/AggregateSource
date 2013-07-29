using System;

namespace AggregateSource.Testing
{
    public interface IAggregateFactoryWhenStateBuilder
    {
        IAggregateFactoryThenStateBuilder Then(params object[] events);
        IAggregateFactoryThrowStateBuilder Throws(Exception exception);
    }
}