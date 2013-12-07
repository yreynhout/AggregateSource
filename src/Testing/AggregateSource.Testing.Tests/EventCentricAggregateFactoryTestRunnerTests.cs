using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    [TestFixture]
    public class EventCentricAggregateFactoryTestRunnerTests
    {
        IEventComparer _comparer;
        EventCentricAggregateFactoryTestRunner _sut;

        [SetUp]
        public void SetUp()
        {
            _comparer = new EqualsEventComparer();
            _sut = new EventCentricAggregateFactoryTestRunner(_comparer);
        }
        [Test]
        public void EventComparerCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EventCentricAggregateFactoryTestRunner(null));
        }

        [Test]
        public void RunSpecificationCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Run(null));
        }

        [Test]
        public void RunReturnsExpectedResultWhenPassed()
        {
            var specification = new EventCentricAggregateFactoryTestSpecification(
                () => new PassCase(),
                new object[0],
                _ => ((PassCase)_).Pass(),
                PassCase.TheExpectedEvents);

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.True);
            Assert.That(result.Failed, Is.False);
            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
        }

        [Test]
        public void RunReturnsExpectedResultWhenFailedBecauseOfNoEvents()
        {
            var specification = new EventCentricAggregateFactoryTestSpecification(
                () => new FailNoEventCase(),
                new object[0],
                _ => ((FailNoEventCase)_).Fail(),
                FailNoEventCase.TheExpectedEvents);

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(new Optional<object[]>(FailNoEventCase.TheActualEvents)));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
        }

        [Test]
        public void RunReturnsExpectedResultWhenFailedBecauseOfDifferentCountOfEvents()
        {
            var specification = new EventCentricAggregateFactoryTestSpecification(
                () => new FailEventCase(),
                new object[0],
                _ => ((FailEventCase)_).Fail(),
                FailEventCase.TheExpectedEvents);

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(new Optional<object[]>(FailEventCase.TheActualEvents)));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
        }

        [Test]
        public void RunReturnsExpectedResultWhenFailedBecauseOfDifferentContentOfEvents()
        {
            var specification = new EventCentricAggregateFactoryTestSpecification(
                () => new FailEventCase(),
                new object[0],
                _ => ((FailEventCase)_).Fail(),
                FailEventCase.TheExpectedEvents);

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(new Optional<object[]>(FailEventCase.TheActualEvents)));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
        }

        [Test]
        public void RunReturnsExpectedResultWhenFailedBecauseExceptionOccurred()
        {
            var specification = new EventCentricAggregateFactoryTestSpecification(
                () => new FailExceptionCase(),
                new object[0],
                _ => ((FailExceptionCase)_).Fail(),
                FailExceptionCase.TheExpectedEvents);

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
            Assert.That(result.ButException, Is.EqualTo(new Optional<Exception>(FailExceptionCase.TheException)));
        }

        class EqualsEventComparer : IEventComparer
        {
            public IEnumerable<EventComparisonDifference> Compare(object expected, object actual)
            {
                if (!expected.Equals(actual))
                    yield return new EventComparisonDifference(expected, actual, "-");
            }
        }

        class FactoryResult : AggregateRootEntity
        {
            public FactoryResult(IEnumerable<object> events)
            {
                foreach (var @event in events)
                {
                    ApplyChange(@event);
                }
            }
        }
        
        class PassCase : AggregateRootEntity
        {
            public static readonly object[] TheExpectedEvents =
            {
                new object()
            };

            public IAggregateRootEntity Pass()
            {
                return new FactoryResult(TheExpectedEvents);
            }
        }

        class FailExceptionCase : AggregateRootEntity
        {
            public static readonly object[] TheExpectedEvents =
            {
                new object()
            };

            public static readonly Exception TheException = new Exception();

            public IAggregateRootEntity Fail()
            {
                throw TheException;
            }
        }

        class FailEventCase : AggregateRootEntity
        {
            public static readonly object[] TheExpectedEvents =
            {
                new object()
            };

            public static readonly object[] TheActualEvents =
            {
                new object()
            };

            public IAggregateRootEntity Fail()
            {
                return new FactoryResult(TheActualEvents);
            }
        }

        class FailNoEventCase : AggregateRootEntity
        {
            public static readonly object[] TheExpectedEvents =
            {
                new object()
            };

            public static readonly object[] TheActualEvents = new object[0];

            public IAggregateRootEntity Fail()
            {
                return new FactoryResult(TheActualEvents);
            }
        }
    }
}