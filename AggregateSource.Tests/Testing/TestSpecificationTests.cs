using System;
using NUnit.Framework;

namespace AggregateSource.Testing {
  [TestFixture]
  public class TestSpecificationTests {
    [Datapoint]
    public Tuple<Guid, object>[] NoEvents = new Tuple<Guid, object>[0];

    [Datapoint]
    public Tuple<Guid, object>[] OneEvent = new [] { new Tuple<Guid, object>(Guid.NewGuid(), new object()) };

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
    public Exception Exception = new Exception();

    [Datapoint]
    public Exception NoException = null;

    [Theory]
    public void UsingDefaultConstructorReturnsInstanceWithExpectedProperties(Tuple<Guid, object>[] givens, object when, Tuple<Guid, object>[] thens, Exception throws) {
      var sut = new TestSpecification(givens, when, thens, throws);

      Assert.That(sut.Givens, Is.EquivalentTo(givens));
      Assert.That(sut.Thens, Is.EquivalentTo(thens));
      Assert.That(sut.When, Is.SameAs(when));
      Assert.That(sut.Throws, Is.SameAs(throws));
    }

    [Theory]
    public void TwoInstancesAreEqualIfTheyHaveTheSameProperties(Tuple<Guid, object>[] givens, object when, Tuple<Guid, object>[] thens, Exception throws) {
      Assert.That(
        new TestSpecification(givens, when, thens, throws),
        Is.EqualTo(new TestSpecification(givens, when, thens, throws)));
    }

    [Theory]
    public void TwoInstancesAreNotEqualIfTheirGivensDiffer(object when, Tuple<Guid, object>[] thens, Exception throws) {
      Assert.That(
        new TestSpecification(new[] { new Tuple<Guid, object>(Guid.NewGuid(), new object()) }, when, thens, throws),
        Is.Not.EqualTo(new TestSpecification(new[] { new Tuple<Guid, object>(Guid.NewGuid(), new object()) }, when, thens, throws)));
    }

    [Theory]
    public void TwoInstancesAreNotEqualIfTheirWhenDiffers(Tuple<Guid, object>[] givens, Tuple<Guid, object>[] thens, Exception throws) {
      Assert.That(
        new TestSpecification(givens, new object(), thens, throws),
        Is.Not.EqualTo(new TestSpecification(givens, new object(), thens, throws)));
    }

    [Theory]
    public void TwoInstancesAreNotEqualIfTheirThensDiffer(Tuple<Guid, object>[] givens, object when, Exception throws) {
      Assert.That(
        new TestSpecification(givens, when, new[] { new Tuple<Guid, object>(Guid.NewGuid(), new object()) }, throws),
        Is.Not.EqualTo(new TestSpecification(givens, when, new[] { new Tuple<Guid, object>(Guid.NewGuid(), new object()) }, throws)));
    }

    [Theory]
    public void TwoInstancesAreNotEqualIfTheirThrowsDiffers(Tuple<Guid, object>[] givens, object when, Tuple<Guid, object>[] thens) {
      Assert.That(
        new TestSpecification(givens, when, thens, new Exception()),
        Is.Not.EqualTo(new TestSpecification(givens, when, thens, new Exception())));
    }

    [Theory]
    public void TwoInstancesHaveTheSameHashCodeIfTheyHaveTheSameProperties(Tuple<Guid, object>[] givens, object when, Tuple<Guid, object>[] thens, Exception throws) {
      Assert.That(
        new TestSpecification(givens, when, thens, throws).GetHashCode(),
        Is.EqualTo(new TestSpecification(givens, when, thens, throws).GetHashCode()));
    }

    [Theory]
    public void TwoInstancesHaveDifferentHashCodeIfTheirGivensDiffer(object when, Tuple<Guid, object>[] thens, Exception throws) {
      Assert.That(
        new TestSpecification(new[] { new Tuple<Guid, object>(Guid.NewGuid(), new object()) }, when, thens, throws).GetHashCode(),
        Is.Not.EqualTo(new TestSpecification(new[] { new Tuple<Guid, object>(Guid.NewGuid(), new object()) }, when, thens, throws).GetHashCode()));
    }

    [Theory]
    public void TwoInstancesHaveDifferentHashCodeIfTheirWhenDiffers(Tuple<Guid, object>[] givens, Tuple<Guid, object>[] thens, Exception throws) {
      Assert.That(
        new TestSpecification(givens, new object(), thens, throws).GetHashCode(),
        Is.Not.EqualTo(new TestSpecification(givens, new object(), thens, throws).GetHashCode()));
    }

    [Theory]
    public void TwoInstancesHaveDifferentHashCodeIfTheirThensDiffer(Tuple<Guid, object>[] givens, object when, Exception throws) {
      Assert.That(
        new TestSpecification(givens, when, new[] { new Tuple<Guid, object>(Guid.NewGuid(), new object()) }, throws).GetHashCode(),
        Is.Not.EqualTo(new TestSpecification(givens, when, new[] { new Tuple<Guid, object>(Guid.NewGuid(), new object()) }, throws).GetHashCode()));
    }

    [Theory]
    public void TwoInstancesHaveDifferentHashCodeIfTheirThrowsDiffers(Tuple<Guid, object>[] givens, object when, Tuple<Guid, object>[] thens) {
      Assert.That(
        new TestSpecification(givens, when, thens, new Exception()).GetHashCode(),
        Is.Not.EqualTo(new TestSpecification(givens, when, thens, new Exception()).GetHashCode()));
    }
  }
}