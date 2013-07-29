using System;

namespace AggregateSource
{
    public static class AggregateStubs
    {
        private static Random _random = new Random();

        public static readonly Aggregate Stub1 =
            Create("Stub/123", AggregateRootEntityStub.Factory());

        public static readonly Aggregate Stub2 =
            Create("Stub/456", AggregateRootEntityStub.Factory());

        public static Aggregate Create<TAggregateRoot>(TAggregateRoot root)
            where TAggregateRoot : AggregateRootEntity
        {
            return new Aggregate("Stub/" + _random.Next(), 0, root);
        }

        public static Aggregate Create<TAggregateRoot>(string identifier, TAggregateRoot root)
            where TAggregateRoot : AggregateRootEntity
        {
            return new Aggregate(identifier, 0, root);
        }
    }
}