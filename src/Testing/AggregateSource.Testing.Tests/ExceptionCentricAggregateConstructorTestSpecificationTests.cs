using System;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    [TestFixture]
    public class ExceptionCentricAggregateConstructorTestSpecificationTests
    {
        ExceptionCentricAggregateConstructorTestSpecification _sut;

        [SetUp]
        public void SetUp()
        {
            Func<IAggregateRootEntity> sutConstructor = () => (IAggregateRootEntity)null;
            var throws = new Exception();

            _sut = new ExceptionCentricAggregateConstructorTestSpecification(
                sutConstructor,
                throws);
        }

        [Test]
        public void SutConstructorCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionCentricAggregateConstructorTestSpecification(
                null,
                new Exception()));
        }

        [Test]
        public void ThrowsCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionCentricAggregateConstructorTestSpecification(
                () => null,
                null));
        }

        [Test]
        public void UsingDefaultCtorReturnsInstanceWithExpectedProperties()
        {
            Func<IAggregateRootEntity> sutConstructor = () => (IAggregateRootEntity)null;
            var throws = new Exception();

            var sut = new ExceptionCentricAggregateConstructorTestSpecification(
                sutConstructor,
                throws);

            Assert.That(sut.SutFactory, Is.SameAs(sutConstructor));
            Assert.That(sut.Throws, Is.SameAs(throws));
        }

        [Test]
        public void PassReturnsExpectedResult()
        {
            var result = _sut.Pass();

            Assert.That(result.Specification, Is.SameAs(_sut));
            Assert.That(result.Passed, Is.True);
            Assert.That(result.Failed, Is.False);
            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
        }

        [Test]
        public void FailReturnsExpectedResult()
        {
            var result = _sut.Fail();

            Assert.That(result.Specification, Is.SameAs(_sut));
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
        }

        [Test]
        public void FailEventsCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Fail((object[])null));
        }

        [Test]
        public void FailEventsReturnsExpectedResult()
        {
            var actual = new[] { new object(), new object() };

            var result = _sut.Fail(actual);

            Assert.That(result.Specification, Is.SameAs(_sut));
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(new Optional<object[]>(actual)));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
        }

        [Test]
        public void FailExceptionCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Fail((Exception)null));
        }

        [Test]
        public void FailExceptionReturnsExpectedResult()
        {
            var actual = new Exception();

            var result = _sut.Fail(actual);

            Assert.That(result.Specification, Is.SameAs(_sut));
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
            Assert.That(result.ButException, Is.EqualTo(new Optional<Exception>(actual)));
        }

    }
}