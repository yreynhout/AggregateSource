using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    [TestFixture]
    public class EventCentricAggregateCommandTestRunnerTests
    {
        IEventComparer _comparer;
        EventCentricAggregateCommandTestRunner _sut;

        [SetUp]
        public void SetUp()
        {
            _comparer = new EqualsEventComparer();
            _sut = new EventCentricAggregateCommandTestRunner(_comparer);
        }
        [Test]
        public void EventComparerCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EventCentricAggregateCommandTestRunner(null));
        }

        [Test]
        public void RunSpecificationCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Run(null));
        }

        [Test]
        public void RunReturnsExpectedResultWhenPassed()
        {
            var specification = new EventCentricAggregateCommandTestSpecification(
                () => new PassCase(), 
                new object[0],
                _ => { },
                new object[0]);

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.True);
            Assert.That(result.Failed, Is.False);
            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
        }

        [Test]
        public void RunReturnsExpectedResultWhenFailedBecauseOfNoEvents()
        {
            var specification = new EventCentricAggregateCommandTestSpecification(
                () => new FailNoEventCase(),
                new object[0],
                _ => ((FailNoEventCase)_).Fail(),
                new [] { new object() });

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(new Optional<object[]>(FailNoEventCase.TheEvents)));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
        }

        [Test]
        public void RunReturnsExpectedResultWhenFailedBecauseOfDifferentCountOfEvents()
        {
            var specification = new EventCentricAggregateCommandTestSpecification(
                () => new FailEventCase(),
                new object[0],
                _ => ((FailEventCase)_).Fail(),
                new object[0]);

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(new Optional<object[]>(FailEventCase.TheEvents)));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
        }

        [Test]
        public void RunReturnsExpectedResultWhenFailedBecauseOfDifferentContentOfEvents()
        {
            var specification = new EventCentricAggregateCommandTestSpecification(
                () => new FailEventCase(),
                new object[0],
                _ => ((FailEventCase)_).Fail(),
                new [] { new object() });

            var result = _sut.Run(specification);
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Failed, Is.True);
            Assert.That(result.ButEvents, Is.EqualTo(new Optional<object[]>(FailEventCase.TheEvents)));
            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
        }

        [Test]
        public void RunReturnsExpectedResultWhenFailedBecauseExceptionOccurred()
        {
            var specification = new EventCentricAggregateCommandTestSpecification(
                () => new FailExceptionCase(), 
                new object[0],
                _ => ((FailExceptionCase)_).Fail(),
                new object[0]);

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
                if(!expected.Equals(actual))
                    yield return new EventComparisonDifference(expected, actual, "-");
            }
        }

        class PassCase : AggregateRootEntity
        {
        }

        class FailExceptionCase : AggregateRootEntity
        {
            public static readonly Exception TheException = new Exception();

            public void Fail()
            {
                throw TheException;
            }
        }

        class FailEventCase : AggregateRootEntity
        {
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

        class FailNoEventCase : AggregateRootEntity
        {
            public static readonly object[] TheEvents = new object[0];

            public void Fail()
            {
                foreach (var theEvent in TheEvents)
                {
                    ApplyChange(theEvent);
                }
            }
        }
    }
}
