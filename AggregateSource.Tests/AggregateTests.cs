using System;
using NUnit.Framework;

namespace AggregateSource.Tests {
  [TestFixture]
  public class AggregateTests {
    [Test]
    public void VersionCanNotBeLowerThanInitialVersion() {
      Assert.Throws<ArgumentOutOfRangeException>(() => 
        new Aggregate(Guid.NewGuid(), Aggregate.InitialVersion - 1, new DummyAggregateRootEntity()));
    }

    [Test]
    public void RootCanNotBeNull() {
      Assert.Throws<ArgumentNullException>(() => 
        new Aggregate(Guid.NewGuid(), Aggregate.InitialVersion, null));
    }

    
    [Test]
    public void UsingDefaultConstructorReturnsInstanceWithExpectedProperties() {
      var id = Guid.NewGuid();
      var version = Aggregate.InitialVersion;
      var root = new DummyAggregateRootEntity();
      var sut = new Aggregate(id, version, root);

      Assert.That(sut.Id, Is.EqualTo(id));
      Assert.That(sut.Version, Is.EqualTo(version));
      Assert.That(sut.Root, Is.EqualTo(root));
    }

    class DummyAggregateRootEntity : AggregateRootEntity {}
  }
}
