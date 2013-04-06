using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource {
  [TestFixture]
  public class AggregateTests {
    [Test, Combinatorial]
    public void UsingDefaultConstructorReturnsInstanceWithExpectedProperties(
      [ValueSource(typeof(AggregateTestsValueSource), "IdSource")]
      string identifier, 
      [Values(Int32.MinValue,-1,0,1,Int32.MaxValue)]
      int version) {
      var root = AggregateRootEntityStub.Factory();
      var sut = new Aggregate(identifier, version, root);

      Assert.That(sut.Identifier, Is.EqualTo(identifier));
      Assert.That(sut.ExpectedVersion, Is.EqualTo(version));
      Assert.That(sut.Root, Is.SameAs(root));
    }

    [Test]
    public void IdentifierCanNotBeNull() {
      Assert.
        Throws<ArgumentNullException>(
          () => new Aggregate(null, 0, AggregateRootEntityStub.Factory()));
    }

    [Test]
    public void RootCanNotBeNull() {
      Assert.
        Throws<ArgumentNullException>(
          () => new Aggregate(Guid.NewGuid().ToString(), 0, null));
    }

    static class AggregateTestsValueSource {
      public static IEnumerable<string> IdSource {
        get {
          yield return Guid.Empty.ToString();
          yield return Guid.NewGuid().ToString();
          yield return "Aggregate/" + Guid.Empty.ToString();
          yield return "Aggregate/" + Guid.NewGuid().ToString();
        }
      }
    }
  }
}
