using System;
using NUnit.Framework;

namespace AggregateSource.Testing {
  public abstract class TestSpecificationDataPointFixture {
    [Datapoint]
    public Tuple<Guid, object>[] NoEvents = new Tuple<Guid, object>[0];

    [Datapoint]
    public Tuple<Guid, object>[] OneEvent = new[] { new Tuple<Guid, object>(Guid.NewGuid(), new object()) };

    [Datapoint]
    public Tuple<Guid, object>[] TwoEventsOfDifferentSources = new[] {
      new Tuple<Guid, object>(Guid.NewGuid(), new object()),
      new Tuple<Guid, object>(Guid.NewGuid(), new object()),
    };

    [Datapoint]
    public Tuple<Guid, object>[] TwoEventsOfTheSameSource = new[] {
      new Tuple<Guid, object>(Guid.Empty, new object()),
      new Tuple<Guid, object>(Guid.Empty, new object()),
    };

    [Datapoint]
    public object Message = new object();

    [Datapoint]
    public Exception Exception = new Exception("Message");
  }
}