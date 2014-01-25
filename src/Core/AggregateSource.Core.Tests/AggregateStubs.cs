using System;

namespace AggregateSource
{
    public static class AggregateStubs
    {
        static readonly Random Random = new Random();

        public static readonly Aggregate Stub1 =
            Create("Stub/123", new AggregateRootEntityStub());

        public static readonly Aggregate Stub2 =
            Create("Stub/456", new AggregateRootEntityStub());

        public static Aggregate Create<TAggregateRoot>(TAggregateRoot root)
            where TAggregateRoot : IAggregateRootEntity
        {
            return new Aggregate("Stub/" + Random.Next(), 0, root);
        }

        public static Aggregate Create<TAggregateRoot>(string identifier, TAggregateRoot root)
            where TAggregateRoot : IAggregateRootEntity
        {
            return new Aggregate(identifier, 0, root);
        }
    }
}