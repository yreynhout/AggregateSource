using System;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    [TestFixture]
    public class ResultCentricAggregateQueryTestSpecificationTests
    {
        ResultCentricAggregateQueryTestSpecification _sut;

        [SetUp]
        public void SetUp()
        {
            Func<IAggregateRootEntity> sutFactory = () => (IAggregateRootEntity)null;
            var givens = new[] { new object(), new object() };
            var result = new object();
            Func<IAggregateRootEntity, object> when = _ => result;
            var then = result;

            _sut = new ResultCentricAggregateQueryTestSpecification(
                sutFactory,
                givens,
                when,
                then);
        }

        [Test]
        public void SutFactoryCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ResultCentricAggregateQueryTestSpecification(
                null,
                new object[0],
                _ => null,
                null));
        }

        [Test]
        public void GivensCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ResultCentricAggregateQueryTestSpecification(
                () => null,
                null,
                _ => null,
                null));
        }

        [Test]
        public void WhenCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ResultCentricAggregateQueryTestSpecification(
                () => null,
                new object[0],
                null,
                null));
        }

        [Test]
        public void ThenCanBeNull()
        {
            Assert.DoesNotThrow(() => new ResultCentricAggregateQueryTestSpecification(
                () => null,
                new object[0],
                _ => null,
                null));
        }

        [Test]
        public void UsingDefaultCtorReturnsInstanceWithExpectedProperties()
        {
            Func<IAggregateRootEntity> sutFactory = () => (IAggregateRootEntity)null;
            var givens = new[] { new object(), new object() };
            var result = new object();
            Func<IAggregateRootEntity, object> when = _ => result;
            var then = result;

            var sut = new ResultCentricAggregateQueryTestSpecification(
                sutFactory,
                givens,
                when,
                then);

            Assert.That(sut.SutFactory, Is.SameAs(sutFactory));
            Assert.That(sut.Givens, Is.EquivalentTo(givens));
            Assert.That(sut.When, Is.SameAs(when));
            Assert.That(sut.Then, Is.SameAs(then));
        }

        [Test]
        public void PassReturnsExpectedResult()
        {
            var result = _sut.Pass();

            Assert.That(result.Specification, Is.SameAs(_sut));
            Assert.That(result.Passed, Is.True);
            Assert.That(result.Failed, Is.False);
            Assert.That(result.ButResult, Is.EqualTo(Optional<object>.Empty));
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
            Assert.That(result.ButResult, Is.EqualTo(Optional<object>.Empty));
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
            Assert.That(result.ButResult, Is.EqualTo(Optional<object>.Empty));
            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
            Assert.That(result.ButException, Is.EqualTo(new Optional<Exception>(actual)));
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
            Assert.That(result.ButResult, Is.EqualTo(new Optional<object>(actual)));
            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
        }
    }
}