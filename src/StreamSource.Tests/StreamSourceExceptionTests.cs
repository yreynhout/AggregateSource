using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace StreamSource
{
    [TestFixture]
    public class StreamSourceExceptionTests
    {
        [Test]
        public void IsAnException()
        {
            Assert.That(new StreamSourceException(), Is.InstanceOf<Exception>());
        }

        [Test]
        public void UsingTheDefaultConstructorReturnsExceptionWithExpectedProperties()
        {
            var sut = new StreamSourceException();

            //Use of Contains.Substring due to differences in OS.
            Assert.That(sut.Message, Contains.Substring(typeof (StreamSourceException).Name));
            Assert.That(sut.InnerException, Is.Null);
        }

        [Test]
        public void UsingTheConstructorWithMessageReturnsExceptionWithExpectedProperties()
        {
            var sut = new StreamSourceException("Message");

            Assert.That(sut.Message, Is.EqualTo("Message"));
            Assert.That(sut.InnerException, Is.Null);
        }

        [Test]
        public void UsingTheConstructorWithMessageAndInnerExceptionReturnsExceptionWithExpectedProperties()
        {
            var innerException = new Exception();
            var sut = new StreamSourceException("Message", innerException);

            Assert.That(sut.Message, Is.EqualTo("Message"));
            Assert.That(sut.InnerException, Is.EqualTo(innerException));
        }

        [Test]
        public void CanBeSerialized()
        {
            var innerException = new Exception("InnerMessage");
            var sut = new StreamSourceException("Message", innerException);

            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, sut);
                stream.Position = 0;
                var result = (StreamSourceException) formatter.Deserialize(stream);

                Assert.That(sut.Message, Is.EqualTo(result.Message));
                Assert.That(sut.InnerException.Message, Is.EqualTo(result.InnerException.Message));
            }
        }
    }
}