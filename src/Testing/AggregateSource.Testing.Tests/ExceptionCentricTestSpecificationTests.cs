using System;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    [TestFixture]
    public class ExceptionCentricTestSpecificationTests : TestSpecificationDataPointFixture
    {
        [Test]
        public void GivensNullThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionCentricTestSpecification(null, Message, Exception));
        }

        [Test]
        public void WhenNullThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionCentricTestSpecification(NoEvents, null, Exception));
        }

        [Test]
        public void ThrowsNullThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionCentricTestSpecification(NoEvents, Message, null));
        }

        [Test]
        public void DoesNotEqualNull()
        {
            var sut = new ExceptionCentricTestSpecification(NoEvents, Message, Exception);

            Assert.That(sut.Equals(null), Is.False);
        }

        [Test]
        public void DoesNotEqualObjectOfOtherType()
        {
            var sut = new ExceptionCentricTestSpecification(NoEvents, Message, Exception);

            Assert.That(sut.Equals(new object()), Is.False);
        }

        [Test]
        public void DoesEqualItself()
        {
            var sut = new ExceptionCentricTestSpecification(NoEvents, Message, Exception);

            Assert.That(sut.Equals((object)sut), Is.True);
        }

        [Theory]
        public void UsingDefaultConstructorReturnsInstanceWithExpectedProperties(Fact[] givens,
                                                                                 object when, Exception throws)
        {
            var sut = new ExceptionCentricTestSpecification(givens, when, throws);

            Assert.That(sut.Givens, Is.EquivalentTo(givens));
            Assert.That(sut.When, Is.SameAs(when));
            Assert.That(sut.Throws, Is.SameAs(throws));
        }

        [Theory]
        public void TwoInstancesAreEqualIfTheyHaveTheSameProperties(Fact[] givens, object when,
                                                                    Exception throws)
        {
            Assert.That(
                new ExceptionCentricTestSpecification(givens, when, throws),
                Is.EqualTo(new ExceptionCentricTestSpecification(givens, when, throws)));
        }

        [Theory]
        public void TwoInstancesAreNotEqualIfTheirGivensDiffer(object when, Exception throws)
        {
            Assert.That(
                new ExceptionCentricTestSpecification(
                    new[] {new Fact(Model.Identifier1, new object())}, when, throws),
                Is.Not.EqualTo(
                    new ExceptionCentricTestSpecification(
                        new[] {new Fact(Model.Identifier1, new object())}, when, throws)));
        }

        [Theory]
        public void TwoInstancesAreNotEqualIfTheirWhenDiffers(Fact[] givens, Exception throws)
        {
            Assert.That(
                new ExceptionCentricTestSpecification(givens, new object(), throws),
                Is.Not.EqualTo(new ExceptionCentricTestSpecification(givens, new object(), throws)));
        }

        [Theory]
        public void TwoInstancesAreNotEqualIfTheirThrowsDiffers(Fact[] givens, object when)
        {
            Assert.That(
                new ExceptionCentricTestSpecification(givens, when, new Exception()),
                Is.Not.EqualTo(new ExceptionCentricTestSpecification(givens, when, new Exception())));
        }

        [Theory]
        public void TwoInstancesHaveTheSameHashCodeIfTheyHaveTheSameProperties(Fact[] givens,
                                                                               object when, Exception throws)
        {
            Assert.That(
                new ExceptionCentricTestSpecification(givens, when, throws).GetHashCode(),
                Is.EqualTo(new ExceptionCentricTestSpecification(givens, when, throws).GetHashCode()));
        }

        [Theory]
        public void TwoInstancesHaveDifferentHashCodeIfTheirGivensDiffer(object when, Exception throws)
        {
            Assert.That(
                new ExceptionCentricTestSpecification(
                    new[] {new Fact(Model.Identifier1, new object())}, when, throws).GetHashCode(),
                Is.Not.EqualTo(
                    new ExceptionCentricTestSpecification(
                        new[] {new Fact(Model.Identifier1, new object())}, when, throws).GetHashCode()));
        }

        [Theory]
        public void TwoInstancesHaveDifferentHashCodeIfTheirWhenDiffers(Fact[] givens, Exception throws)
        {
            Assert.That(
                new ExceptionCentricTestSpecification(givens, new object(), throws).GetHashCode(),
                Is.Not.EqualTo(new ExceptionCentricTestSpecification(givens, new object(), throws).GetHashCode()));
        }

        [Theory]
        public void TwoInstancesHaveDifferentHashCodeIfTheirThrowsDiffers(Fact[] givens, object when)
        {
            Assert.That(
                new ExceptionCentricTestSpecification(givens, when, new Exception()).GetHashCode(),
                Is.Not.EqualTo(new ExceptionCentricTestSpecification(givens, when, new Exception()).GetHashCode()));
        }

        [Test]
        public void PassReturnsExpectedResult()
        {
            var sut = new ExceptionCentricTestSpecification(NoEvents, Message, Exception);
            var result = sut.Pass();

            Assert.That(result.Specification, Is.SameAs(sut));
            Assert.That(result.Passed, Is.True);
            Assert.That(result.Failed, Is.False);
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
        }

        [Test]
        public void FailReturnsExpectedResult()
        {
            var sut = new ExceptionCentricTestSpecification(NoEvents, Message, Exception);
            var result = sut.Fail();

            Assert.That(result.Specification, Is.SameAs(sut));
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
        }

        [Test]
        public void FailWithNullExceptionThrows()
        {
            var sut = new ExceptionCentricTestSpecification(NoEvents, Message, Exception);

            Assert.Throws<ArgumentNullException>(() => { var _ = sut.Fail((Exception) null); });
        }

        [Test]
        public void FailWithActualExceptionReturnsExpectedResult()
        {
            var sut = new ExceptionCentricTestSpecification(NoEvents, Message, Exception);
            var actual = new Exception();

            var result = sut.Fail(actual);

            Assert.That(result.Specification, Is.SameAs(sut));
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButException, Is.EqualTo(new Optional<Exception>(actual)));
        }

        [Test]
        public void FailWithNullEventsThrows()
        {
            var sut = new ExceptionCentricTestSpecification(NoEvents, Message, Exception);

            Assert.Throws<ArgumentNullException>(() => { var _ = sut.Fail((Fact[]) null); });
        }

        [Test]
        public void FailWithActualEventsReturnsExpectedResult()
        {
            var sut = new ExceptionCentricTestSpecification(NoEvents, Message, Exception);

            var actual = new[] {new Fact(Model.Identifier1, new object())};

            var result = sut.Fail(actual);

            Assert.That(result.Specification, Is.SameAs(sut));
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(new Optional<Fact[]>(actual)));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
        }
    }
}