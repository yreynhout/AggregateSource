//using System;
//using System.Collections.Generic;
//using NUnit.Framework;

//namespace AggregateSource.Testing
//{
//    [TestFixture]
//    public class ExceptionCentricAggregateCommandTestRunnerTests
//    {
//        IExceptionComparer _comparer;
//        ExceptionCentricAggregateCommandTestRunner _sut;

//        [SetUp]
//        public void SetUp()
//        {
//            _comparer = new EqualsExceptionComparer();
//            _sut = new ExceptionCentricAggregateCommandTestRunner(_comparer);
//        }
//        [Test]
//        public void EventComparerCanNotBeNull()
//        {
//            Assert.Throws<ArgumentNullException>(() => new ExceptionCentricAggregateCommandTestRunner(null));
//        }

//        [Test]
//        public void RunSpecificationCanNotBeNull()
//        {
//            Assert.Throws<ArgumentNullException>(() => _sut.Run(null));
//        }

//        [Test]
//        public void RunReturnsExpectedResultWhenPassed()
//        {
//            var specification = new ExceptionCentricAggregateCommandTestSpecification(
//                () => new PassCase(),
//                new object[0],
//                _ => ((PassCase)_).Pass(),
//                PassCase.TheException);

//            var result = _sut.Run(specification);
//            Assert.That(result.Passed, Is.True);
//            Assert.That(result.Failed, Is.False);
//            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
//            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
//        }

//        [Test]
//        public void RunReturnsExpectedResultWhenFailedBecauseOfEvents()
//        {
//            var specification = new ExceptionCentricAggregateCommandTestSpecification(
//                () => new FailEventCase(),
//                new object[0],
//                _ => ((FailEventCase)_).Fail(),
//                new object[0]);

//            var result = _sut.Run(specification);
//            Assert.That(result.Passed, Is.False);
//            Assert.That(result.Failed, Is.True);
//            Assert.That(result.ButEvents, Is.EqualTo(new Optional<object[]>(FailEventCase.TheEvents)));
//            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
//        }

//        [Test]
//        public void RunReturnsExpectedResultWhenFailedBecauseOfDifferentContentOfEvents()
//        {
//            var specification = new ExceptionCentricAggregateCommandTestSpecification(
//                () => new FailEventCase(),
//                new object[0],
//                _ => ((FailEventCase)_).Fail(),
//                new[] { new object() });

//            var result = _sut.Run(specification);
//            Assert.That(result.Passed, Is.False);
//            Assert.That(result.Failed, Is.True);
//            Assert.That(result.ButEvents, Is.EqualTo(new Optional<object[]>(FailEventCase.TheEvents)));
//            Assert.That(result.ButException, Is.EqualTo(Optional<Exception>.Empty));
//        }

//        [Test]
//        public void RunReturnsExpectedResultWhenFailedBecauseExceptionOccurred()
//        {
//            var specification = new ExceptionCentricAggregateCommandTestSpecification(
//                () => new FailExceptionCase(),
//                new object[0],
//                _ => ((FailExceptionCase)_).Fail(),
//                new object[0]);

//            var result = _sut.Run(specification);
//            Assert.That(result.Passed, Is.False);
//            Assert.That(result.Failed, Is.True);
//            Assert.That(result.ButEvents, Is.EqualTo(Optional<object[]>.Empty));
//            Assert.That(result.ButException, Is.EqualTo(new Optional<Exception>(FailExceptionCase.TheException)));
//        }

//        class EqualsExceptionComparer : IExceptionComparer
//        {
//            public IEnumerable<ExceptionComparisonDifference> Compare(Exception expected, Exception actual)
//            {
//                if (!expected.Equals(actual))
//                    yield return new ExceptionComparisonDifference(expected, actual, "-");
//            }
//        }

//        class PassCase : AggregateRootEntity
//        {
//            public static readonly Exception TheException = new Exception();

//            public void Pass()
//            {
//                throw TheException;
//            }
//        }

//        class FailExceptionCase : AggregateRootEntity
//        {
//            public static readonly Exception TheException = new Exception();

//            public void Fail()
//            {
//                throw TheException;
//            }
//        }

//        class FailEventCase : AggregateRootEntity
//        {
//            public static readonly object[] TheEvents =
//            {
//                new object()
//            };

//            public void Fail()
//            {
//                foreach (var theEvent in TheEvents)
//                {
//                    Apply(theEvent);
//                }
//            }
//        }

//        class FailNoEventCase : AggregateRootEntity
//        {
//            public static readonly object[] TheEvents = new object[0];

//            public void Fail()
//            {
//                foreach (var theEvent in TheEvents)
//                {
//                    Apply(theEvent);
//                }
//            }
//        }
//    }
//}