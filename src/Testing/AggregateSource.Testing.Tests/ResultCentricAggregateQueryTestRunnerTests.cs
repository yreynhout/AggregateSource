using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    [TestFixture]
    public class ResultCentricAggregateQueryTestRunnerTests
    {
        IResultComparer _comparer;
        ResultCentricAggregateQueryTestRunner _sut;

        [SetUp]
        public void SetUp()
        {
            _comparer = new EqualsResultComparer();
            _sut = new ResultCentricAggregateQueryTestRunner(_comparer);
        }
        [Test]
        public void EventComparerCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ResultCentricAggregateQueryTestRunner(null));
        }

        [Test]
        public void RunSpecificationCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Run(null));
        }

        [Test]
        public void RunReturnsExpectedResultWhenPassed()
        {
            var specification = new ResultCentricAggregateQueryTestSpecification(
                () => new PassCase(),
                new object[0],
                _ => ((PassCase)_).Pass(),
                PassCase.TheResult);

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.True);
            Assert.That(result.Failed, Is.False);
            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
            Assert.That(result.ButResult, Is.EqualTo(Optional<int>.Empty));
        }

        [Test]
        public void RunReturnsExpectedResultWhenFailedBecauseOfEvents()
        {
            var specification = new ResultCentricAggregateQueryTestSpecification(
                () => new FailEventCase(),
                new object[0],
                _ => ((FailEventCase)_).Fail(),
                FailEventCase.TheResult);

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(new Optional<object[]>(FailEventCase.TheEvents)));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
            Assert.That(result.ButResult, Is.EqualTo(Optional<int>.Empty));
        }

        [Test]
        public void RunReturnsExpectedResultWhenFailedBecauseExceptionOccurred()
        {
            var specification = new ResultCentricAggregateQueryTestSpecification(
                () => new FailExceptionCase(),
                new object[0],
                _ => ((FailExceptionCase)_).Fail(),
                FailExceptionCase.TheResult);

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
            Assert.That(result.ButException, Is.EqualTo(new Optional<Exception>(FailExceptionCase.TheException)));
            Assert.That(result.ButResult, Is.EqualTo(Optional<int>.Empty));
        }

        [Test]
        public void RunReturnsExpectedResultWhenFailedBecauseDifferentResult()
        {
            var specification = new ResultCentricAggregateQueryTestSpecification(
                () => new FailResultCase(), 
                new object[0],
                _ => ((FailResultCase)_).Fail(),
                FailResultCase.TheExpectedResult);

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
            Assert.That(result.ButResult, Is.EqualTo(new Optional<int>(FailResultCase.TheActualResult)));
        }

        class EqualsResultComparer : IResultComparer
        {
            public IEnumerable<ResultComparisonDifference> Compare(object expected, object actual)
            {
                if (!expected.Equals(actual))
                    yield return new ResultComparisonDifference(expected, actual, "-");
            }
        }

        class PassCase : AggregateRootEntity
        {
            public static readonly int TheResult = 1;
            public int Pass()
            {
                return TheResult;
            }
        }

        class FailExceptionCase : AggregateRootEntity
        {
            public static readonly int TheResult = 1;
            public static readonly Exception TheException = new Exception();

            public int Fail()
            {
                throw TheException;
            }
        }

        class FailEventCase : AggregateRootEntity
        {
            public static readonly int TheResult = 1;
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

        class FailResultCase : AggregateRootEntity
        {
            public static readonly int TheExpectedResult = 1;
            public static readonly int TheActualResult = 0;

            public int Fail()
            {
                return TheActualResult;
            }
        }
    }
}