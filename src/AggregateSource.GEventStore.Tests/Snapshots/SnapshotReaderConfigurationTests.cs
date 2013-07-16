using System;
using FakeItEasy;
using NUnit.Framework;

namespace AggregateSource.GEventStore.Snapshots {
  [TestFixture]
  public class SnapshotReaderConfigurationTests {
    [Test]
    public void ResolverCannotBeNull() {
      Assert.Throws<ArgumentNullException>(() => new SnapshotReaderConfiguration(null, A.Fake<ISnapshotDeserializer>()));
    }

    [Test]
    public void DeserializerCannotBeNull() {
      Assert.Throws<ArgumentNullException>(() => new SnapshotReaderConfiguration(A.Fake<IStreamNameResolver>(), null));
    }

    [Test]
    public void UsingConstructorReturnsInstanceWithExpectedProperties() {
      var resolver = A.Fake<IStreamNameResolver>();
      var deserializer = A.Fake<ISnapshotDeserializer>();

      var sut = new SnapshotReaderConfiguration(resolver, deserializer);

      Assert.That(sut.Resolver, Is.SameAs(resolver));
      Assert.That(sut.Deserializer, Is.SameAs(deserializer));
    }
  }
}
