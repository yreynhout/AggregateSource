using System;
using NUnit.Framework;

namespace AggregateSource.Testing {
  [TestFixture]
  public class EventCentricTestSpecificationTests : TestSpecificationDataPointFixture {
    [Test]
    public void GivensNullThrows() {
      Assert.Throws<ArgumentNullException>(() => new EventCentricTestSpecification(null, Message, NoEvents));
    }

    [Test]
    public void WhenNullThrows() {
      Assert.Throws<ArgumentNullException>(() => new EventCentricTestSpecification(NoEvents, null, NoEvents));
    }

    [Test]
    public void ThenNullThrows() {
      Assert.Throws<ArgumentNullException>(() => new EventCentricTestSpecification(NoEvents, Message, null));
    }

    [Test]
    public void DoesNotEqualNull() {
      var sut = new EventCentricTestSpecification(NoEvents, Message, NoEvents);

      Assert.That(sut.Equals(null), Is.False);
    }

    [Test]
    public void DoesNotEqualObjectOfOtherType() {
      var sut = new EventCentricTestSpecification(NoEvents, Message, NoEvents);

      Assert.That(sut.Equals(new object()), Is.False);
    }

    [Test]
    public void DoesEqualItself() {
      var sut = new EventCentricTestSpecification(NoEvents, Message, NoEvents);

      Assert.That(sut.Equals(sut), Is.True);
    }

    [Theory]
    public void UsingDefaultConstructorReturnsInstanceWithExpectedProperties(Tuple<Guid, object>[] givens, object when, Tuple<Guid, object>[] thens) {
      var sut = new EventCentricTestSpecification(givens, when, thens);

      Assert.That(sut.Givens, Is.EquivalentTo(givens));
      Assert.That(sut.When, Is.SameAs(when));
      Assert.That(sut.Thens, Is.EquivalentTo(thens));
    }

    [Theory]
    public void TwoInstancesAreEqualIfTheyHaveTheSameProperties(Tuple<Guid, object>[] givens, object when, Tuple<Guid, object>[] thens) {
      Assert.That(
        new EventCentricTestSpecification(givens, when, thens),
        Is.EqualTo(new EventCentricTestSpecification(givens, when, thens)));
    }

    [Theory]
    public void TwoInstancesAreNotEqualIfTheirGivensDiffer(object when, Tuple<Guid, object>[] thens) {
      Assert.That(
        new EventCentricTestSpecification(new[] { new Tuple<Guid, object>(Guid.NewGuid(), new object()) }, when, thens),
        Is.Not.EqualTo(new EventCentricTestSpecification(new[] { new Tuple<Guid, object>(Guid.NewGuid(), new object()) }, when, thens)));
    }

    [Theory]
    public void TwoInstancesAreNotEqualIfTheirWhenDiffers(Tuple<Guid, object>[] givens, Tuple<Guid, object>[] thens) {
      Assert.That(
        new EventCentricTestSpecification(givens, new object(), thens),
        Is.Not.EqualTo(new EventCentricTestSpecification(givens, new object(), thens)));
    }

    [Theory]
    public void TwoInstancesAreNotEqualIfTheirThensDiffer(Tuple<Guid, object>[] givens, object when) {
      Assert.That(
        new EventCentricTestSpecification(givens, when, new[] { new Tuple<Guid, object>(Guid.NewGuid(), new object()) }),
        Is.Not.EqualTo(new EventCentricTestSpecification(givens, when, new[] { new Tuple<Guid, object>(Guid.NewGuid(), new object()) })));
    }

    [Theory]
    public void TwoInstancesHaveTheSameHashCodeIfTheyHaveTheSameProperties(Tuple<Guid, object>[] givens, object when, Tuple<Guid, object>[] thens) {
      Assert.That(
        new EventCentricTestSpecification(givens, when, thens).GetHashCode(),
        Is.EqualTo(new EventCentricTestSpecification(givens, when, thens).GetHashCode()));
    }

    [Theory]
    public void TwoInstancesHaveDifferentHashCodeIfTheirGivensDiffer(object when, Tuple<Guid, object>[] thens) {
      Assert.That(
        new EventCentricTestSpecification(new[] { new Tuple<Guid, object>(Guid.NewGuid(), new object()) }, when, thens).GetHashCode(),
        Is.Not.EqualTo(new EventCentricTestSpecification(new[] { new Tuple<Guid, object>(Guid.NewGuid(), new object()) }, when, thens).GetHashCode()));
    }

    [Theory]
    public void TwoInstancesHaveDifferentHashCodeIfTheirWhenDiffers(Tuple<Guid, object>[] givens, Tuple<Guid, object>[] thens, Exception throws) {
      Assert.That(
        new EventCentricTestSpecification(givens, new object(), thens).GetHashCode(),
        Is.Not.EqualTo(new EventCentricTestSpecification(givens, new object(), thens).GetHashCode()));
    }

    [Theory]
    public void TwoInstancesHaveDifferentHashCodeIfTheirThensDiffer(Tuple<Guid, object>[] givens, object when) {
      Assert.That(
        new EventCentricTestSpecification(givens, when, new[] { new Tuple<Guid, object>(Guid.NewGuid(), new object()) }).GetHashCode(),
        Is.Not.EqualTo(new EventCentricTestSpecification(givens, when, new[] { new Tuple<Guid, object>(Guid.NewGuid(), new object()) }).GetHashCode()));
    }

    [Test]
    public void PassReturnsExpectedResult() {
      var sut = new EventCentricTestSpecification(NoEvents, Message, NoEvents);
      var result = sut.Pass();

      Assert.That(result.Specification, Is.SameAs(sut));
      Assert.That(result.Passed, Is.True);
      Assert.That(result.Failed, Is.False);
      Assert.That(result.Buts, Is.EqualTo(Optional<Exception>.Empty));
    }

    [Test]
    public void FailReturnsExpectedResult() {
      var sut = new EventCentricTestSpecification(NoEvents, Message, NoEvents);
      var result = sut.Fail();

      Assert.That(result.Specification, Is.SameAs(sut));
      Assert.That(result.Passed, Is.False);
      Assert.That(result.Failed, Is.True);
      Assert.That(result.Buts, Is.EqualTo(Optional<Exception>.Empty));
    }

    [Test]
    public void FailWithNullActualThrows() {
      var sut = new EventCentricTestSpecification(NoEvents, Message, NoEvents);

      Assert.Throws<ArgumentNullException>(() => { var _ = sut.Fail(null); });
    }

    [Test]
    public void FailWithActualReturnsExpectedResult() {
      var sut = new EventCentricTestSpecification(NoEvents, Message, NoEvents);

      var actual = new[] { new Tuple<Guid, object>(Guid.NewGuid(), new object()) };

      var result = sut.Fail(actual);

      Assert.That(result.Specification, Is.SameAs(sut));
      Assert.That(result.Passed, Is.False);
      Assert.That(result.Failed, Is.True);
      Assert.That(result.Buts, Is.EqualTo(new Optional<Tuple<Guid, object>[]>(actual)));
    }
  }
}