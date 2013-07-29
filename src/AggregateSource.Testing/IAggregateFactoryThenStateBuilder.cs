namespace AggregateSource.Testing
{
    public interface IAggregateFactoryThenStateBuilder
    {
        IAggregateFactoryThenStateBuilder Then(params object[] events);
        EventCentricAggregateFactoryTestSpecification Build();
    }
}