using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    [TestFixture]
    public class ExceptionCentricAggregateConstructorTestRunnerTests
    {
        IExceptionComparer _comparer;
        ExceptionCentricAggregateConstructorTestRunner _sut;

        [SetUp]
        public void SetUp()
        {
            _comparer = new EqualsExceptionComparer();
            _sut = new ExceptionCentricAggregateConstructorTestRunner(_comparer);
        }

        [Test]
        public void EventComparerCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionCentricAggregateConstructorTestRunner(null));
        }

        [Test]
        public void RunSpecificationCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Run(null));
        }

        [Test]
        public void RunReturnsExpectedResultWhenPassed()
        {
            var specification = new ExceptionCentricAggregateConstructorTestSpecification(
                () => new PassCase(),
                PassCase.TheException);

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.True);
            Assert.That(result.Failed, Is.False);
            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
        }

        [Test]
        public void RunReturnsExpectedResultWhenFailedBecauseOfEvents()
        {
            var specification = new ExceptionCentricAggregateConstructorTestSpecification(
                () => new FailEventCase(),
                FailEventCase.TheExpectedException);

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(new Optional<object[]>(FailEventCase.TheEvents)));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
        }

        [Test]
        public void RunReturnsExpectedResultWhenFailedBecauseOfDifferentException()
        {
            var specification = new ExceptionCentricAggregateConstructorTestSpecification(
                () => new FailExceptionCase(),
                FailExceptionCase.TheExpectedException);

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
            Assert.That(result.ButException, Is.EqualTo(new Optional<Exception>(FailExceptionCase.TheActualException)));
        }

        [Test]
        public void RunReturnsExpectedResultWhenFailedBecauseNoExceptionOccurred()
        {
            var specification = new ExceptionCentricAggregateConstructorTestSpecification(
                () => new FailNoExceptionCase(),
                FailNoExceptionCase.TheExpectedException);

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
        }

        class EqualsExceptionComparer : IExceptionComparer
        {
            public IEnumerable<ExceptionComparisonDifference> Compare(Exception expected, Exception actual)
            {
                if (!expected.Equals(actual))
                    yield return new ExceptionComparisonDifference(expected, actual, "-");
            }
        }

        class PassCase : AggregateRootEntity
        {
            public static readonly Exception TheException = new Exception();

            public PassCase()
            {
                throw TheException;
            }
        }

        class FailExceptionCase : AggregateRootEntity
        {
            public static readonly Exception TheExpectedException = new Exception();
            public static readonly Exception TheActualException = new Exception();

            public FailExceptionCase()
            {
                throw TheActualException;
            }
        }

        class FailEventCase : AggregateRootEntity
        {
            public static readonly Exception TheExpectedException = new Exception();

            public static readonly object[] TheEvents =
            {
                new object()
            };

            public FailEventCase()
            {
                foreach (var theEvent in TheEvents)
                {
                    ApplyChange(theEvent);
                }
            }
        }

        class FailNoExceptionCase : AggregateRootEntity
        {
            public static readonly Exception TheExpectedException = new Exception();
        }
    }
}