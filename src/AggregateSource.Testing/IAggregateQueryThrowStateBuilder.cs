namespace AggregateSource.Testing
{
    public interface IAggregateQueryThrowStateBuilder
    {
        ExceptionCentricAggregateQueryTestSpecification Build();
    }
}