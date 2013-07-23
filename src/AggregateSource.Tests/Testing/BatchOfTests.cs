using System;
using NUnit.Framework;

namespace AggregateSource.Testing {
  namespace BatchOfTests {
    [TestFixture]
    public class InitialInstanceTests {
      private BatchOf _sut;

      [SetUp]
      public void SetUp() {
        _sut = new BatchOf();
      }

      [Test]
      public void IsEmpty() {
        Tuple<string, object>[] result = _sut;

        Assert.That(result, Is.Empty);
      }
    }

    //[TestFixture]
    //public class InstanceWithOneFactTests {
    //  private BatchOf _sut;

    //  [SetUp]
    //  public void SetUp() {
    //    _sut = new BatchOf();
    //  }

    //  [Test]
    //  public void IsEmpty() {
    //    Tuple<string, object>[] result = _sut;

    //    Assert.That(result, Is.Empty);
    //  }
    //}
  }
}

namespace AggregateSource.Testing {
  public class BatchOf {
    public static implicit operator Tuple<string, object>[](BatchOf instance) {
      return new Tuple<string, object>[0];
    }
  }
}
