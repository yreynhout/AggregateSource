using System;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    [TestFixture]
    public class EventCentricAggregateConstructorTestSpecificationTests
    {
        EventCentricAggregateConstructorTestSpecification _sut;

        [SetUp]
        public void SetUp()
        {
            Func<IAggregateRootEntity> sutFactory = () => (IAggregateRootEntity)null;
            var thens = new[] { new object(), new object() };

            _sut = new EventCentricAggregateConstructorTestSpecification(
                sutFactory,
                thens);
        }

        [Test]
        public void SutFactoryCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EventCentricAggregateConstructorTestSpecification(
                                                           null,
                                                           new object[0]));
        }

        [Test]
        public void ThensCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EventCentricAggregateConstructorTestSpecification(
                                                           () => null,
                                                           null));
        }

        [Test]
        public void UsingDefaultCtorReturnsInstanceWithExpectedProperties()
        {
            Func<IAggregateRootEntity> sutFactory = () => (IAggregateRootEntity)null;
            var thens = new[] { new object(), new object() };

            var sut = new EventCentricAggregateConstructorTestSpecification(
                sutFactory,
                thens);

            Assert.That(sut.SutFactory, Is.SameAs(sutFactory));
            Assert.That(sut.Thens, Is.EquivalentTo(thens));
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