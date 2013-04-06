using NUnit.Framework;

namespace StreamSource {
  [TestFixture]
  public class ExpectedVersionTests {
    [Test]
    public void ExpectedVersionNoneIsMinus1() {
      Assert.That(ExpectedVersion.None, Is.EqualTo(-1));
    }
  }
}
