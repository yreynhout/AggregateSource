using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource.GEventStore.Snapshots {
  [TestFixture]
  public class SnapshotTests {
    [Test, Combinatorial]
    public void UsingConstructorReturnsInstanceWithExpectedProperties(
      [Values(Int32.MinValue,-1,0,1,Int32.MaxValue)]
      int version,
      [ValueSource("StateObjects")]
      object state) {
      var sut = new Snapshot(version, state);

      Assert.That(sut.Version, Is.EqualTo(version));
      Assert.That(sut.State, Is.EqualTo(state));
    }

    IEnumerable<object> StateObjects {
      get {
        yield return null;
        yield return new object();
      }
    }

    [Test]
    public void DoesNotEqualNull() {
      Assert.That(CreateSut(), Is.Not.EqualTo(null));
    }

    [Test]
    public void TwoInstancesAreEqualIfTheyHaveTheSameVersionAndState() {
      Assert.That(
          CreateSut(),
          Is.EqualTo(CreateSut()));
    }

    [Test]
    public void TwoInstancesAreEqualIfTheyHaveTheSameVersionAndStateIsNullForBoth() {
      Assert.That(
          CreateSutWithState(null),
          Is.EqualTo(CreateSutWithState(null)));
    }

    [Test]
    public void TwoInstancesAreNotEqualIfTheirVersionDiffers() {
      Assert.That(
          CreateSutWithVersion(1),
          Is.Not.EqualTo(CreateSutWithVersion(2)));
    }
    
    [Test]
    public void TwoInstancesAreNotEqualIfTheirStateDiffers() {
      Assert.That(
          CreateSutWithState(new object()),
          Is.Not.EqualTo(CreateSutWithState(new object())));
    }

    [Test]
    public void TwoInstancesHaveTheSameHashCodeIfTheyHaveTheSameVersionAndState() {
      Assert.That(
          CreateSut().GetHashCode(),
          Is.EqualTo(CreateSut().GetHashCode()));
    }

    [Test]
    public void TwoInstancesHaveTheSameHashCodeIfTheyHaveTheSameVersionAndStateIsNullForBoth() {
      Assert.That(
          CreateSutWithState(null).GetHashCode(),
          Is.EqualTo(CreateSutWithState(null).GetHashCode()));
    }
    
    [Test]
    public void TwoInstancesHaveDifferentHashCodesIfTheirVersionDiffers() {
      Assert.That(
          CreateSutWithVersion(1).GetHashCode(),
          Is.Not.EqualTo(CreateSutWithVersion(2).GetHashCode()));
    }

    [Test]
    public void TwoInstancesHaveDifferentHashCodesIfTheirStateDiffers() {
      Assert.That(
          CreateSutWithState(new object()).GetHashCode(),
          Is.Not.EqualTo(CreateSutWithState(new object()).GetHashCode()));
    }

    static readonly object KnownState = new object();
    const int KnownVersion = 0;

    static Snapshot CreateSut() {
      return CreateSut(KnownVersion, KnownState);
    }

    static Snapshot CreateSutWithVersion(int version) {
      return CreateSut(version, KnownState);
    }

    static Snapshot CreateSutWithState(object state) {
      return CreateSut(KnownVersion, state);
    }

    static Snapshot CreateSut(int version, object state) {
      return new Snapshot(version, state);
    }
  }
}
