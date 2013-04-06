using System;
using NUnit.Framework;

namespace AggregateSource.Testing {
  public abstract class TestSpecificationDataPointFixture {
    [Datapoint]
    public Tuple<string, object>[] NoEvents = new Tuple<string, object>[0];

    [Datapoint]
    public Tuple<string, object>[] OneEvent = new[] { new Tuple<string, object>("Aggregate/" + Guid.NewGuid(), new object()) };

    [Datapoint]
    public Tuple<string, object>[] TwoEventsOfDifferentSources = new[] {
      new Tuple<string, object>("Aggregate/" + Guid.NewGuid(), new object()),
      new Tuple<string, object>("Aggregate/" + Guid.NewGuid(), new object()),
    };

    [Datapoint]
    public Tuple<string, object>[] TwoEventsOfTheSameSource = new[] {
      new Tuple<string, object>("Aggregate/" + new Guid("C8F75337-62BA-41F0-B57B-10171388FD6F"), new object()),
      new Tuple<string, object>("Aggregate/" + new Guid("C8F75337-62BA-41F0-B57B-10171388FD6F"), new object()),
    };

    [Datapoint]
    public object Message = new object();

    [Datapoint]
    public Exception Exception = new Exception("Message");
  }
}