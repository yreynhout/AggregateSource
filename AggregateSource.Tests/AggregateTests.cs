using System;
using NUnit.Framework;

namespace AggregateSource.Tests {
  [TestFixture]
  public class AggregateTests {
    [Test]
    public void RootCanNotBeNull() {
      Assert.Throws<ArgumentNullException>(() => 
        new Aggregate(Guid.NewGuid(), null));
    }

    
    [Test]
    public void UsingDefaultConstructorReturnsInstanceWithExpectedProperties() {
      var id = Guid.NewGuid();
      var root = new DummyAggregateRootEntity();
      var sut = new Aggregate(id, root);

      Assert.That(sut.Id, Is.EqualTo(id));
      Assert.That(sut.Root, Is.EqualTo(root));
    }

    class DummyAggregateRootEntity : AggregateRootEntity {}
  }
}
