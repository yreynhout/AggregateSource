using System;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    [TestFixture]
    public class ExceptionCentricAggregateQueryTestSpecificationTests
    {
        ExceptionCentricAggregateQueryTestSpecification _sut;

        [SetUp]
        public void SetUp()
        {
            Func<IAggregateRootEntity> sutQuery = () => (IAggregateRootEntity)null;
            var givens = new[] { new object(), new object() };
            Func<IAggregateRootEntity, object> when = _ => null;
            var throws = new Exception();

            _sut = new ExceptionCentricAggregateQueryTestSpecification(
                sutQuery,
                givens,
                when,
                throws);
        }

        [Test]
        public void SutQueryCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionCentricAggregateQueryTestSpecification(
                null,
                new object[0],
                _ => null,
                new Exception()));
        }

        [Test]
        public void GivensCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionCentricAggregateQueryTestSpecification(
                () => null,
                null,
                _ => null,
                new Exception()));
        }

        [Test]
        public void WhenCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionCentricAggregateQueryTestSpecification(
                () => null,
                new object[0],
                null,
                new Exception()));
        }

        [Test]
        public void ThrowsCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionCentricAggregateQueryTestSpecification(
                () => null,
                new object[0],
                _ => null,
                null));
        }

        [Test]
        public void UsingDefaultCtorReturnsInstanceWithExpectedProperties()
        {
            Func<IAggregateRootEntity> sutQuery = () => (IAggregateRootEntity)null;
            var givens = new[] { new object(), new object() };
            Func<IAggregateRootEntity, object> when = _ => null;
            var throws = new Exception();

            var sut = new ExceptionCentricAggregateQueryTestSpecification(
                sutQuery,
                givens,
                when,
                throws);

            Assert.That(sut.SutFactory, Is.SameAs(sutQuery));
            Assert.That(sut.Givens, Is.EquivalentTo(givens));
            Assert.That(sut.When, Is.SameAs(when));
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
            Assert.That(result.ButResult, Is.EqualTo(Optional<object>.Empty));
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
            Assert.That(result.ButResult, Is.EqualTo(Optional<object>.Empty));
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
            Assert.That(result.ButResult, Is.EqualTo(Optional<object>.Empty));
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
            Assert.That(result.ButResult, Is.EqualTo(Optional<object>.Empty));
        }

        [Test]
        public void FailResultCanBeNull()
        {
            Assert.DoesNotThrow(() => _sut.Fail((object)null));
        }

        [Test]
        public void FailResultReturnsExpectedResult()
        {
            var actual = new object();

            var result = _sut.Fail(actual);

            Assert.That(result.Specification, Is.SameAs(_sut));
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
            Assert.That(result.ButResult, Is.EqualTo(new Optional<object>(actual)));
        }

    }
}