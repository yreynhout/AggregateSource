using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    [TestFixture]
    public class ExceptionCentricAggregateCommandTestRunnerTests
    {
        IExceptionComparer _comparer;
        ExceptionCentricAggregateCommandTestRunner _sut;

        [SetUp]
        public void SetUp()
        {
            _comparer = new EqualsExceptionComparer();
            _sut = new ExceptionCentricAggregateCommandTestRunner(_comparer);
        }

        [Test]
        public void EventComparerCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionCentricAggregateCommandTestRunner(null));
        }

        [Test]
        public void RunSpecificationCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Run(null));
        }

        [Test]
        public void RunReturnsExpectedResultWhenPassed()
        {
            var specification = new ExceptionCentricAggregateCommandTestSpecification(
                () => new PassCase(),
                new object[0],
                _ => ((PassCase)_).Pass(),
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
            var specification = new ExceptionCentricAggregateCommandTestSpecification(
                () => new FailEventCase(),
                new object[0],
                _ => ((FailEventCase)_).Fail(),
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
            var specification = new ExceptionCentricAggregateCommandTestSpecification(
                () => new FailExceptionCase(),
                new object[0],
                _ => ((FailExceptionCase)_).Fail(),
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
            var specification = new ExceptionCentricAggregateCommandTestSpecification(
                () => new FailNoExceptionCase(),
                new object[0],
                _ => ((FailNoExceptionCase)_).Fail(),
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

            public void Pass()
            {
                throw TheException;
            }
        }

        class FailExceptionCase : AggregateRootEntity
        {
            public static readonly Exception TheExpectedException = new Exception();
            public static readonly Exception TheActualException = new Exception();

            public void Fail()
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

            public void Fail()
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

            public void Fail()
            {
            }
        }
    }
}