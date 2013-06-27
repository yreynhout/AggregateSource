using System;
using FakeItEasy;
using NUnit.Framework;

namespace AggregateSource.GEventStore.Snapshots {
  [TestFixture]
  public class SnapshotStoreReadConfigurationTests {
    [Test]
    public void ResolverCannotBeNull() {
      Assert.Throws<ArgumentNullException>(() => new SnapshotStoreReadConfiguration(null, A.Fake<ISnapshotDeserializer>()));
    }

    [Test]
    public void DeserializerCannotBeNull() {
      Assert.Throws<ArgumentNullException>(() => new SnapshotStoreReadConfiguration(A.Fake<IStreamNameResolver>(), null));
    }

    [Test]
    public void UsingConstructorReturnsInstanceWithExpectedProperties() {
      var resolver = A.Fake<IStreamNameResolver>();
      var deserializer = A.Fake<ISnapshotDeserializer>();

      var sut = new SnapshotStoreReadConfiguration(resolver, deserializer);

      Assert.That(sut.Resolver, Is.SameAs(resolver));
      Assert.That(sut.Deserializer, Is.SameAs(deserializer));
    }
  }
}
