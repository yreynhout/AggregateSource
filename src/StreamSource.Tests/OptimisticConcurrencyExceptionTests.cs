using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace StreamSource
{
    [TestFixture]
    public class OptimisticConcurrencyExceptionTests
    {
        static readonly Guid StreamId = Guid.NewGuid();
        const Int32 ExpectedVersion = 123;

        [Test]
        public void IsStreamSourceException()
        {
            Assert.That(new OptimisticConcurrencyException(StreamId, ExpectedVersion),
                        Is.InstanceOf<StreamSourceException>());
        }

        [Test]
        public void UsingConstructorWithStreamIdAndExpectedVersionReturnsExceptionWithExcpectedProperties()
        {
            var sut = new OptimisticConcurrencyException(StreamId, ExpectedVersion);
            Assert.That(sut.StreamId, Is.EqualTo(StreamId));
            Assert.That(sut.ExpectedVersion, Is.EqualTo(ExpectedVersion));
            //Use of Contains.Substring due to differences in OS.
            Assert.That(sut.Message, Contains.Substring(typeof (OptimisticConcurrencyException).Name));
            Assert.That(sut.InnerException, Is.Null);
        }

        [Test]
        public void UsingConstructorWithStreamIdAndExpectedVersionAndMessageReturnsExceptionWithExcpectedProperties()
        {
            var sut = new OptimisticConcurrencyException(StreamId, ExpectedVersion, "Message");
            Assert.That(sut.StreamId, Is.EqualTo(StreamId));
            Assert.That(sut.ExpectedVersion, Is.EqualTo(ExpectedVersion));
            Assert.That(sut.Message, Is.EqualTo("Message"));
            Assert.That(sut.InnerException, Is.Null);
        }

        [Test]
        public void
            UsingConstructorWithStreamIdAndExpectedVersionAndMessageAndInnerExceptionReturnsExceptionWithExcpectedProperties
            ()
        {
            var exception = new Exception();
            var sut = new OptimisticConcurrencyException(StreamId, ExpectedVersion, "Message", exception);
            Assert.That(sut.StreamId, Is.EqualTo(StreamId));
            Assert.That(sut.ExpectedVersion, Is.EqualTo(ExpectedVersion));
            Assert.That(sut.Message, Is.EqualTo("Message"));
            Assert.That(sut.InnerException, Is.SameAs(exception));
        }

        [Test]
        public void CanBeSerialized()
        {
            var innerException = new Exception("InnerMessage");
            var sut = new OptimisticConcurrencyException(StreamId, ExpectedVersion, "Message", innerException);

            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, sut);
                stream.Position = 0;
                var result = (OptimisticConcurrencyException) formatter.Deserialize(stream);

                Assert.That(sut.StreamId, Is.EqualTo(StreamId));
                Assert.That(sut.ExpectedVersion, Is.EqualTo(ExpectedVersion));
                Assert.That(sut.Message, Is.EqualTo(result.Message));
                Assert.That(sut.InnerException.Message, Is.EqualTo(result.InnerException.Message));
            }
        }
    }
}