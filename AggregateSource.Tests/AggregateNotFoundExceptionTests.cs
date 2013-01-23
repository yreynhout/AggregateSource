using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace AggregateSource.Tests {
  [TestFixture]
  public class AggregateNotFoundExceptionTests {
    [Test]
    public void IsAnAggregateSourceException() {
      Assert.That(new AggregateNotFoundException(Guid.Empty, typeof(object)), Is.InstanceOf<AggregateSourceException>());
    }

    [Test]
    public void UsingTheDefaultContstructorReturnsExceptionWithExpectedProperties() {
      var aggregateId = Guid.NewGuid();
      var aggregateType = typeof (Object);
      var sut = new AggregateNotFoundException(aggregateId, aggregateType);

      Assert.That(sut.AggregateId, Is.EqualTo(aggregateId));
      Assert.That(sut.AggregateType, Is.EqualTo(aggregateType));
      Assert.That(sut.Message, Is.EqualTo(
        "The Object aggregate with identifier " + aggregateId + " could not be found. Please make sure the callsite is indeed passing in an identifier for an Object aggregate."));
      Assert.That(sut.InnerException, Is.Null);
    }

    [Test]
    public void UsingTheDefaultContstructorAggregateTypeCannotBeNull() {
      Assert.Throws<ArgumentNullException>(() => new AggregateNotFoundException(Guid.Empty, null));
    }

    [Test]
    public void UsingTheConstructorWithMessageReturnsExceptionWithExpectedProperties() {
      var aggregateId = Guid.NewGuid();
      var aggregateType = typeof(Object);
      var sut = new AggregateNotFoundException(aggregateId, aggregateType, "Message");

      Assert.That(sut.AggregateId, Is.EqualTo(aggregateId));
      Assert.That(sut.AggregateType, Is.EqualTo(aggregateType));
      Assert.That(sut.Message, Is.EqualTo("Message"));
      Assert.That(sut.InnerException, Is.Null);
    }

    [Test]
    public void UsingTheConstructorWithMessageAggregateTypeCannotBeNull() {
      Assert.Throws<ArgumentNullException>(() => new AggregateNotFoundException(Guid.Empty, null));
    }

    [Test]
    public void UsingTheConstructorWithMessageAndInnerExceptionReturnsExceptionWithExpectedProperties() {
      var innerException = new Exception();
      var aggregateId = Guid.NewGuid();
      var aggregateType = typeof(Object);
      var sut = new AggregateNotFoundException(aggregateId, aggregateType, "Message", innerException);

      Assert.That(sut.AggregateId, Is.EqualTo(aggregateId));
      Assert.That(sut.AggregateType, Is.EqualTo(aggregateType));
      Assert.That(sut.Message, Is.EqualTo("Message"));
      Assert.That(sut.InnerException, Is.EqualTo(innerException));
    }

    [Test]
    public void UsingTheConstructorWithMessageAndInnerExceptionAggregateTypeCannotBeNull() {
      Assert.Throws<ArgumentNullException>(() => new AggregateNotFoundException(Guid.Empty, null));
    }

    [Test]
    public void CanBeSerialized() {
      var innerException = new Exception("InnerMessage");
      var aggregateId = Guid.NewGuid();
      var aggregateType = typeof(Object);
      var sut = new AggregateNotFoundException(aggregateId, aggregateType, "Message", innerException);

      using (var stream = new MemoryStream()) {
        var formatter = new BinaryFormatter();
        formatter.Serialize(stream, sut);
        stream.Position = 0;
        var result = (AggregateNotFoundException)formatter.Deserialize(stream);

        Assert.That(sut.AggregateId, Is.EqualTo(aggregateId));
        Assert.That(sut.AggregateType, Is.EqualTo(aggregateType));
        Assert.That(sut.Message, Is.EqualTo(result.Message));
        Assert.That(sut.InnerException.Message, Is.EqualTo(result.InnerException.Message));
      }
    }

    //TODO: Test that AggregateType could not be resolved upon deserialization.
  }
}