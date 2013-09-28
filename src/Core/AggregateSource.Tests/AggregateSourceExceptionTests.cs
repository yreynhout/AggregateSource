using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace AggregateSource
{
    [TestFixture]
    public class AggregateSourceExceptionTests
    {
        [Test]
        public void IsAnException()
        {
            Assert.That(new AggregateSourceException(), Is.InstanceOf<Exception>());
        }

        [Test]
        public void UsingTheDefaultConstructorReturnsExceptionWithExpectedProperties()
        {
            var sut = new AggregateSourceException();

            //Use of Contains.Substring due to differences in OS.
            Assert.That(sut.Message, Contains.Substring(typeof (AggregateSourceException).Name));
            Assert.That(sut.InnerException, Is.Null);
        }

        [Test]
        public void UsingTheConstructorWithMessageReturnsExceptionWithExpectedProperties()
        {
            var sut = new AggregateSourceException("Message");

            Assert.That(sut.Message, Is.EqualTo("Message"));
            Assert.That(sut.InnerException, Is.Null);
        }

        [Test]
        public void UsingTheConstructorWithMessageAndInnerExceptionReturnsExceptionWithExpectedProperties()
        {
            var innerException = new Exception();
            var sut = new AggregateSourceException("Message", innerException);

            Assert.That(sut.Message, Is.EqualTo("Message"));
            Assert.That(sut.InnerException, Is.EqualTo(innerException));
        }

        [Test]
        public void CanBeSerialized()
        {
            var innerException = new Exception("InnerMessage");
            var sut = new AggregateSourceException("Message", innerException);

            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, sut);
                stream.Position = 0;
                var result = (AggregateSourceException) formatter.Deserialize(stream);

                Assert.That(sut.Message, Is.EqualTo(result.Message));
                Assert.That(sut.InnerException.Message, Is.EqualTo(result.InnerException.Message));
            }
        }
    }
}