using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;

namespace AggregateSource {
  namespace OptionalTests {
    [TestFixture]
    public class WithEmptyInstance {
      Optional<AggregateRootEntityStub> _sut;

      [SetUp]
      public void SetUp() {
        _sut = Optional<AggregateRootEntityStub>.Empty;
      }

      [Test]
      public void HasValueReturnsFalse() {
        Assert.That(_sut.HasValue, Is.False);
      }

      [Test]
      public void ValueThrows() {
        Assert.Throws<InvalidOperationException>(() => { var value = _sut.Value; });
      }

      [Test]
      public void TypedEnumerationIsEmpty() {
        Assert.That(_sut, Is.EquivalentTo(Enumerable.Empty<AggregateRootEntityStub>()));
      }

      [Test]
      public void ObjectEnumerationIsEmpty() {
        var sut = (IEnumerable) _sut;

        Assert.That(sut, Is.EquivalentTo(Enumerable.Empty<AggregateRootEntityStub>()));
      }

      [Test]
      public void DoesNotEqualNull() {
        Assert.IsFalse(_sut.Equals(null));
      }

      [Test]
      public void DoesNotEqualObjectOfOtherType() {
        Assert.IsFalse(_sut.Equals(new object()));
      }

      [Test]
      public void DoesEqualItself() {
        Assert.IsTrue(_sut.Equals(_sut));
      }

      [Test]
      public void TwoInstancesAreEqualIfTheyBothDoNotHaveAValueAndAreOfTheSameValueType() {
        Assert.IsTrue(_sut.Equals(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void TwoInstancesAreNotEqualIfTheOtherHasAValueAndIsOfTheSameValueType() {
        Assert.IsFalse(_sut.Equals(new Optional<AggregateRootEntityStub>(AggregateRootEntityStub.Factory())));
      }

      [Test]
      public void TwoInstanceHaveTheSameHashCodeIfTheyBothDoNotHaveAValueAndAreOfTheSameValueType() {
        Assert.IsTrue(_sut.GetHashCode().Equals(Optional<AggregateRootEntityStub>.Empty.GetHashCode()));
      }

      [Test]
      public void TwoInstancesDoNotHaveTheSameHashCodeIfTheOtherHasAValueAndIsOfTheSameValueType() {
        Assert.IsFalse(_sut.GetHashCode().Equals(new Optional<AggregateRootEntityStub>(AggregateRootEntityStub.Factory()).GetHashCode()));
      }
    }

    [TestFixture]
    public class WithFilledInstance {
      Optional<AggregateRootEntityStub> _sut;
      AggregateRootEntityStub _instance;

      [SetUp]
      public void SetUp() {
        _instance = AggregateRootEntityStub.Factory();
        _sut = new Optional<AggregateRootEntityStub>(_instance);
      }

      [Test]
      public void HasValueReturnsTrue() {
        Assert.That(_sut.HasValue, Is.True);
      }

      [Test]
      public void ValueReturnsInitializationInstance() {
        Assert.That(_sut.Value, Is.SameAs(_instance));
      }

      [Test]
      public void TypedEnumerationReturnsInstance() {
        Assert.That(_sut, Is.EquivalentTo(new [] { _instance }));
      }

      [Test]
      public void ObjectEnumerationReturnsInstance() {
        var sut = (IEnumerable)_sut;

        Assert.That(sut, Is.EquivalentTo(new object[] { _instance }));
      }

      [Test]
      public void DoesNotEqualNull() {
        Assert.IsFalse(_sut.Equals(null));
      }

      [Test]
      public void DoesNotEqualObjectOfOtherType() {
        Assert.IsFalse(_sut.Equals(new object()));
      }

      [Test]
      public void DoesEqualItself() {
        Assert.IsTrue(_sut.Equals(_sut));
      }

      [Test]
      public void TwoInstancesAreEqualIfTheyBothHaveTheSameValue() {
        Assert.IsTrue(_sut.Equals(new Optional<AggregateRootEntityStub>(_instance)));
      }

      [Test]
      public void TwoInstancesAreNotEqualIfTheOtherHasNoValue() {
        Assert.IsFalse(_sut.Equals(Optional<AggregateRootEntityStub>.Empty));
      }

      [Test]
      public void TwoInstancesAreNotEqualIfTheOtherHasDifferentValue() {
        Assert.IsFalse(_sut.Equals(new Optional<AggregateRootEntityStub>(AggregateRootEntityStub.Factory())));
      }

      [Test]
      public void TwoInstancesHaveTheSameHashCodeIfTheyBothHaveTheSameValue() {
        Assert.IsTrue(_sut.GetHashCode().Equals(new Optional<AggregateRootEntityStub>(_instance).GetHashCode()));
      }

      [Test]
      public void TwoInstancesDoNotHaveTheSameHashCodeIfTheOtherHasNoValue() {
        Assert.IsFalse(_sut.GetHashCode().Equals(Optional<AggregateRootEntityStub>.Empty.GetHashCode()));
      }

      [Test]
      public void TwoInstancesDoNotHaveTheSameHashCodeIfTheOtherHasDifferentValue() {
        Assert.IsFalse(_sut.GetHashCode().Equals(new Optional<AggregateRootEntityStub>(AggregateRootEntityStub.Factory()).GetHashCode()));
      }
    }
  }
}
