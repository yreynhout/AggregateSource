using System;

namespace AggregateSource
{
    public class AggregateRootEntityStub : AggregateRootEntity
    {
        public static readonly Func<AggregateRootEntityStub> Factory = () => new AggregateRootEntityStub();
    }
}