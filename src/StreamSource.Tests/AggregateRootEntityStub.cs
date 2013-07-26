using System;
using AggregateSource;

namespace StreamSource
{
    public class AggregateRootEntityStub : AggregateRootEntity
    {
        public static readonly Func<AggregateRootEntityStub> Factory = () => new AggregateRootEntityStub();
    }
}