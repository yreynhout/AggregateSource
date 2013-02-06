using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource.Tests {
  [TestFixture]
  public class AggregateTests {
    [Test, Combinatorial]
    public void UsingDefaultConstructorReturnsInstanceWithExpectedProperties(
      [ValueSource(typeof(AggregateTestsValueSource), "IdSource")]
      Guid id, 
      [Values(Int32.MinValue,-1,0,1,Int32.MaxValue)]
      int version) {
      var root = AggregateRootEntityStub.Factory();
      var sut = new Aggregate(id, version, root);

      Assert.That(sut.Id, Is.EqualTo(id));
      Assert.That(sut.ExpectedVersion, Is.EqualTo(version));
      Assert.That(sut.Root, Is.SameAs(root));
    }

    [Test]
    public void RootCanNotBeNull() {
      Assert.Throws<ArgumentNullException>(() =>
        new Aggregate(Guid.NewGuid(), 0, null));
    }

    static class AggregateTestsValueSource {
      public static IEnumerable<Guid> IdSource {
        get {
          yield return Guid.Empty;
          yield return Guid.NewGuid();
        }
      }
    }
  }
}
