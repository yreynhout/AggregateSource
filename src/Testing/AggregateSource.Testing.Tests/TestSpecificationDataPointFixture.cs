using System;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    public abstract class TestSpecificationDataPointFixture
    {
        [Datapoint]
        public static Fact[] NoEvents = new Fact[0];

        [Datapoint]
        public static Fact[] OneEvent = new[] { new Fact("Aggregate/" + Guid.NewGuid(), new object()) };

        [Datapoint]
        public static Fact[] TwoEventsOfDifferentSources = new[]
        {
            new Fact("Aggregate/" + Guid.NewGuid(), new object()),
            new Fact("Aggregate/" + Guid.NewGuid(), new object())
        };

        [Datapoint]
        public static Fact[] TwoEventsOfTheSameSource = new[]
        {
            new Fact("Aggregate/" + new Guid("C8F75337-62BA-41F0-B57B-10171388FD6F"), new object()),
            new Fact("Aggregate/" + new Guid("C8F75337-62BA-41F0-B57B-10171388FD6F"), new object())
        };

        [Datapoint] public static object Message = new object();

        [Datapoint] public static Exception Exception = new Exception("Message");
    }
}