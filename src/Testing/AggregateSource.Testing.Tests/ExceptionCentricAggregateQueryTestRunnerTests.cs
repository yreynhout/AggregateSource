using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    [TestFixture]
    public class ExceptionCentricAggregateQueryTestRunnerTests
    {
        IExceptionComparer _comparer;
        ExceptionCentricAggregateQueryTestRunner _sut;

        [SetUp]
        public void SetUp()
        {
            _comparer = new EqualsExceptionComparer();
            _sut = new ExceptionCentricAggregateQueryTestRunner(_comparer);
        }

        [Test]
        public void EventComparerCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionCentricAggregateQueryTestRunner(null));
        }

        [Test]
        public void RunSpecificationCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Run(null));
        }

        [Test]
        public void RunReturnsExpectedResultWhenPassed()
        {
            var specification = new ExceptionCentricAggregateQueryTestSpecification(
                () => new PassCase(),
                new object[0],
                _ => ((PassCase)_).Pass(),
                PassCase.TheException);

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.True);
            Assert.That(result.Failed, Is.False);
            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
            Assert.That(result.ButResult, Is.EqualTo(Optional<object>.Empty));
        }

        [Test]
        public void RunReturnsExpectedResultWhenFailedBecauseOfEvents()
        {
            var specification = new ExceptionCentricAggregateQueryTestSpecification(
                () => new FailEventCase(),
                new object[0],
                _ => ((FailEventCase)_).Fail(),
                FailEventCase.TheExpectedException);

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(new Optional<object[]>(FailEventCase.TheEvents)));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
            Assert.That(result.ButResult, Is.EqualTo(Optional<object>.Empty));
        }

        [Test]
        public void RunReturnsExpectedResultWhenFailedBecauseOfDifferentException()
        {
            var specification = new ExceptionCentricAggregateQueryTestSpecification(
                () => new FailExceptionCase(),
                new object[0],
                _ => ((FailExceptionCase)_).Fail(),
                FailExceptionCase.TheExpectedException);

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
            Assert.That(result.ButException, Is.EqualTo(new Optional<Exception>(FailExceptionCase.TheActualException)));
            Assert.That(result.ButResult, Is.EqualTo(Optional<object>.Empty));
        }

        [Test]
        public void RunReturnsExpectedResultWhenFailedBecauseNoExceptionOccurred()
        {
            var specification = new ExceptionCentricAggregateQueryTestSpecification(
                () => new FailNoExceptionCase(),
                new object[0],
                _ => ((FailNoExceptionCase)_).Fail(),
                FailNoExceptionCase.TheExpectedException);

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
            Assert.That(result.ButResult, Is.EqualTo(new Optional<object>(FailNoExceptionCase.TheResult)));
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

            public int Pass()
            {
                throw TheException;
            }
        }

        class FailExceptionCase : AggregateRootEntity
        {
            public static readonly Exception TheExpectedException = new Exception();
            public static readonly Exception TheActualException = new Exception();

            public int Fail()
            {
                throw TheActualException;
            }
        }

        class FailEventCase : AggregateRootEntity
        {
            public static readonly int TheResult = 1;
            public static readonly Exception TheExpectedException = new Exception();

            public static readonly object[] TheEvents =
            {
                new object()
            };

            public int Fail()
            {
                foreach (var theEvent in TheEvents)
                {
                    ApplyChange(theEvent);
                }
                return TheResult;
            }
        }

        class FailNoExceptionCase : AggregateRootEntity
        {
            public static readonly int TheResult = 1;
            public static readonly Exception TheExpectedException = new Exception();

            public int Fail()
            {
                return TheResult;
            }
        }
    }
}