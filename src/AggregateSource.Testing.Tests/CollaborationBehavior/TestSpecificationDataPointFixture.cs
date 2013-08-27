using System;
using NUnit.Framework;

namespace AggregateSource.Testing.CollaborationBehavior
{
    public abstract class TestSpecificationDataPointFixture
    {
        [Datapoint]
        public Fact[] NoEvents = new Fact[0];

        [Datapoint]
        public Fact[] OneEvent = new[] { new Fact("Aggregate/" + Guid.NewGuid(), new object()) };

        [Datapoint]
        public Fact[] TwoEventsOfDifferentSources = new[]
        {
            new Fact("Aggregate/" + Guid.NewGuid(), new object()),
            new Fact("Aggregate/" + Guid.NewGuid(), new object())
        };

        [Datapoint]
        public Fact[] TwoEventsOfTheSameSource = new[]
        {
            new Fact("Aggregate/" + new Guid("C8F75337-62BA-41F0-B57B-10171388FD6F"), new object()),
            new Fact("Aggregate/" + new Guid("C8F75337-62BA-41F0-B57B-10171388FD6F"), new object())
        };

        [Datapoint] public object Message = new object();

        [Datapoint] public Exception Exception = new Exception("Message");
    }
}