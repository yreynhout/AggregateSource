using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace AggregateSource
{
    [TestFixture]
    public class AggregateNotFoundExceptionTests
    {
        private static readonly string AggregateIdentifier = Guid.NewGuid().ToString();
        private static readonly Type AggregateType = typeof (object);

        [Test]
        public void IsAnAggregateSourceException()
        {
            Assert.That(new AggregateNotFoundException(AggregateIdentifier, AggregateType),
                        Is.InstanceOf<AggregateSourceException>());
        }

        [Test]
        public void UsingTheDefaultContstructorReturnsExceptionWithExpectedProperties()
        {
            var sut = new AggregateNotFoundException(AggregateIdentifier, AggregateType);

            Assert.That(sut.Identifier, Is.EqualTo(AggregateIdentifier));
            Assert.That(sut.Type, Is.EqualTo(AggregateType));
            Assert.That(sut.Message, Is.EqualTo(
                "The Object aggregate with identifier " + AggregateIdentifier +
                " could not be found. Please make sure the callsite is indeed passing in an identifier for an Object aggregate."));
            Assert.That(sut.InnerException, Is.Null);
        }

        [Test]
        public void UsingTheDefaultContstructorAggregateIdentifierCannotBeNull()
        {
            Assert.That(
                Assert.Throws<ArgumentNullException>(() => new AggregateNotFoundException(null, AggregateType))
                      .ParamName,
                Is.EqualTo("identifier"));
        }

        [Test]
        public void UsingTheDefaultContstructorAggregateTypeCannotBeNull()
        {
            Assert.That(
                Assert.Throws<ArgumentNullException>(() => new AggregateNotFoundException(AggregateIdentifier, null))
                      .ParamName,
                Is.EqualTo("type"));
        }

        [Test]
        public void UsingTheConstructorWithMessageReturnsExceptionWithExpectedProperties()
        {
            var sut = new AggregateNotFoundException(AggregateIdentifier, AggregateType, "Message");

            Assert.That(sut.Identifier, Is.EqualTo(AggregateIdentifier));
            Assert.That(sut.Type, Is.EqualTo(AggregateType));
            Assert.That(sut.Message, Is.EqualTo("Message"));
            Assert.That(sut.InnerException, Is.Null);
        }

        [Test]
        public void UsingTheConstructorWithMessageAggregateIdentifierCannotBeNull()
        {
            Assert.That(
                Assert.Throws<ArgumentNullException>(
                    () => new AggregateNotFoundException(null, AggregateType, "Message"))
                      .ParamName,
                Is.EqualTo("identifier"));
        }

        [Test]
        public void UsingTheConstructorWithMessageAggregateTypeCannotBeNull()
        {
            Assert.That(
                Assert.Throws<ArgumentNullException>(
                    () => new AggregateNotFoundException(AggregateIdentifier, null, "Message"))
                      .ParamName,
                Is.EqualTo("type"));
        }

        [Test]
        public void UsingTheConstructorWithMessageAndInnerExceptionReturnsExceptionWithExpectedProperties()
        {
            var innerException = new Exception();
            var sut = new AggregateNotFoundException(AggregateIdentifier, AggregateType, "Message", innerException);

            Assert.That(sut.Identifier, Is.EqualTo(AggregateIdentifier));
            Assert.That(sut.Type, Is.EqualTo(AggregateType));
            Assert.That(sut.Message, Is.EqualTo("Message"));
            Assert.That(sut.InnerException, Is.EqualTo(innerException));
        }

        [Test]
        public void UsingTheConstructorWithMessageAndInnerExceptionAggregateIdentifierCannotBeNull()
        {
            Assert.That(
                Assert.Throws<ArgumentNullException>(
                    () => new AggregateNotFoundException(null, AggregateType, "Message", new Exception())).ParamName,
                Is.EqualTo("identifier"));
        }

        [Test]
        public void UsingTheConstructorWithMessageAndInnerExceptionAggregateTypeCannotBeNull()
        {
            Assert.That(
                Assert.Throws<ArgumentNullException>(
                    () => new AggregateNotFoundException(AggregateIdentifier, null, "Message", new Exception()))
                      .ParamName,
                Is.EqualTo("type"));
        }

        [Test]
        public void CanBeSerialized()
        {
            var innerException = new Exception("InnerMessage");
            var sut = new AggregateNotFoundException(AggregateIdentifier, AggregateType, "Message", innerException);

            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, sut);
                stream.Position = 0;
                var result = (AggregateNotFoundException) formatter.Deserialize(stream);

                Assert.That(sut.Identifier, Is.EqualTo(AggregateIdentifier));
                Assert.That(sut.Type, Is.EqualTo(AggregateType));
                Assert.That(sut.Message, Is.EqualTo(result.Message));
                Assert.That(sut.InnerException.Message, Is.EqualTo(result.InnerException.Message));
            }
        }

        //TODO: Test that type could not be resolved upon deserialization.
    }
}