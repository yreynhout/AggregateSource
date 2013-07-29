namespace AggregateSource.Testing
{
    public interface IAggregateFactoryThrowStateBuilder
    {
        ExceptionCentricAggregateFactoryTestSpecification Build();
    }
}