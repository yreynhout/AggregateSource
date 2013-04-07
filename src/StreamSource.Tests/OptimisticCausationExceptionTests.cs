using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace StreamSource {
  [TestFixture]
  public class OptimisticCausationExceptionTests {
    private static readonly Guid CausationId = Guid.NewGuid();

    [Test]
    public void IsStreamSourceException() {
      Assert.That(new OptimisticCausationException(CausationId), Is.InstanceOf<StreamSourceException>());
    }

    [Test]
    public void UsingConstructorWithCausationIdReturnsExceptionWithExcpectedProperties() {
      var sut = new OptimisticCausationException(CausationId);
      Assert.That(sut.CausationId, Is.EqualTo(CausationId));
      //Use of Contains.Substring due to differences in OS.
      Assert.That(sut.Message, Contains.Substring(typeof(OptimisticCausationException).Name));
      Assert.That(sut.InnerException, Is.Null);
    }

    [Test]
    public void UsingConstructorWithCausationIdAndMessageReturnsExceptionWithExcpectedProperties() {
      var sut = new OptimisticCausationException(CausationId, "Message");
      Assert.That(sut.CausationId, Is.EqualTo(CausationId));
      Assert.That(sut.Message, Is.EqualTo("Message"));
      Assert.That(sut.InnerException, Is.Null);
    }

    [Test]
    public void UsingConstructorWithCausationIdAndMessageAndInnerExceptionReturnsExceptionWithExcpectedProperties() {
      var exception = new Exception();
      var sut = new OptimisticCausationException(CausationId, "Message", exception);
      Assert.That(sut.CausationId, Is.EqualTo(CausationId));
      Assert.That(sut.Message, Is.EqualTo("Message"));
      Assert.That(sut.InnerException, Is.SameAs(exception));
    }


    [Test]
    public void CanBeSerialized() {
      var innerException = new Exception("InnerMessage");
      var sut = new OptimisticCausationException(CausationId, "Message", innerException);

      using (var stream = new MemoryStream()) {
        var formatter = new BinaryFormatter();
        formatter.Serialize(stream, sut);
        stream.Position = 0;
        var result = (OptimisticCausationException)formatter.Deserialize(stream);

        Assert.That(sut.CausationId, Is.EqualTo(CausationId));
        Assert.That(sut.Message, Is.EqualTo(result.Message));
        Assert.That(sut.InnerException.Message, Is.EqualTo(result.InnerException.Message));
      }
    }
  }
}
