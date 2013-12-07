using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    namespace NUnitExtensionsForCommandScenarioTests
    {
        [TestFixture]
        public class EventCentricAssert
        {
            [Test]
            public void BuilderCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(
                    () => ((IEventCentricAggregateCommandTestSpecificationBuilder)null).Assert(null));
            }

            [Test]
            public void ComparerCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(
                    () => NullBuilder.Instance.Assert(null));
            }

            [Test]
            public void WhenSpecificationRunPasses()
            {
                Assert.DoesNotThrow(
                    () =>
                        new CommandScenarioFor<PassCase>(() => new PassCase()).
                            GivenNone().
                            When(_ => _.Pass()).
                            Then(PassCase.TheExpectedEvents).
                            Assert(new EqualsEventComparer()));
            }

            [Test]
            public void WhenSpecificationRunFailsWithEventCountDifference()
            {
                Assert.Throws<AssertionException>(
                    () =>
                        new CommandScenarioFor<FailEventCountDifferenceCase>(() => new FailEventCountDifferenceCase()).
                            GivenNone().
                            When(_ => _.Fail()).
                            Then(FailEventCountDifferenceCase.TheExpectedEvents).
                            Assert(new EqualsEventComparer()));
            }

            [Test]
            public void WhenSpecificationRunFailsWithException()
            {
                Assert.Throws<AssertionException>(
                    () =>
                        new CommandScenarioFor<FailExceptionCase>(() => new FailExceptionCase()).
                            GivenNone().
                            When(_ => _.Fail()).
                            Then(FailExceptionCase.TheExpectedEvents).
                            Assert(new EqualsEventComparer()));
            }

            [Test]
            public void WhenSpecificationRunFailsWithEventDifferences()
            {
                Assert.Throws<AssertionException>(
                    () =>
                        new CommandScenarioFor<FailEventDifferenceCase>(() => new FailEventDifferenceCase()).
                            GivenNone().
                            When(_ => _.Fail()).
                            Then(FailEventDifferenceCase.TheExpectedEvents).
                            Assert(new EqualsEventComparer()));
            }

            class EqualsEventComparer : IEventComparer
            {
                public IEnumerable<EventComparisonDifference> Compare(object expected, object actual)
                {
                    if (!expected.Equals(actual))
                        yield return new EventComparisonDifference(expected, actual, "-");
                }
            }

            class PassCase : AggregateRootEntity
            {
                public static readonly object[] TheExpectedEvents =
                {
                    new object()
                };

                public void Pass()
                {
                    foreach (var theEvent in TheExpectedEvents)
                    {
                        ApplyChange(theEvent);
                    }
                }
            }

            class FailExceptionCase : AggregateRootEntity
            {
                public static readonly object[] TheExpectedEvents =
                {
                    new object()
                };

                public static readonly Exception TheException = new Exception();

                public void Fail()
                {
                    throw TheException;
                }
            }

            class FailEventCountDifferenceCase : AggregateRootEntity
            {
                public static readonly object[] TheExpectedEvents = new object[0];

                public static readonly object[] TheActualEvents =
                {
                    new object()
                };

                public void Fail()
                {
                    foreach (var theEvent in TheActualEvents)
                    {
                        ApplyChange(theEvent);
                    }
                }
            }

            class FailEventDifferenceCase : AggregateRootEntity
            {
                public static readonly object[] TheExpectedEvents =
                {
                    new object()
                };

                public static readonly object[] TheActualEvents =
                {
                    new object()
                };

                public void Fail()
                {
                    foreach (var theEvent in TheActualEvents)
                    {
                        ApplyChange(theEvent);
                    }
                }
            }

            class NullBuilder : IEventCentricAggregateCommandTestSpecificationBuilder
            {
                public static readonly IEventCentricAggregateCommandTestSpecificationBuilder Instance = new NullBuilder();

                public EventCentricAggregateCommandTestSpecification Build()
                {
                    return null;
                }
            }
        }

        [TestFixture]
        public class ExceptionCentricAssert
        {
            [Test]
            public void BuilderCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(
                    () => ((IExceptionCentricAggregateCommandTestSpecificationBuilder)null).Assert(new EqualsExceptionComparer()));
            }

            [Test]
            public void ComparerCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(
                    () => NullBuilder.Instance.Assert(null));
            }

            [Test]
            public void WhenSpecificationRunPasses()
            {
                Assert.DoesNotThrow(
                    () =>
                        new CommandScenarioFor<PassCase>(() => new PassCase()).
                        GivenNone().
                        When(_ => _.Pass()).
                        Throws(PassCase.TheException).
                        Assert(new EqualsExceptionComparer()));
            }

            [Test]
            public void WhenSpecificationRunFailsBecauseDifferentException()
            {
                Assert.Throws<AssertionException>(
                    () =>
                        new CommandScenarioFor<FailExceptionCase>(() => new FailExceptionCase()).
                            GivenNone().
                            When(_ => _.Fail()).
                            Throws(FailExceptionCase.TheExpectedException).
                            Assert(new EqualsExceptionComparer()));
            }

            [Test]
            public void WhenSpecificationRunFailsBecauseEvents()
            {
                Assert.Throws<AssertionException>(
                    () =>
                        new CommandScenarioFor<FailEventCase>(() => new FailEventCase()).
                            GivenNone().
                            When(_ => _.Fail()).
                            Throws(FailEventCase.TheExpectedException).
                            Assert(new EqualsExceptionComparer()));
            }

            [Test]
            public void WhenSpecificationRunFailsBecauseNoException()
            {
                Assert.Throws<AssertionException>(
                    () =>
                        new CommandScenarioFor<FailNoExceptionCase>(() => new FailNoExceptionCase()).
                            GivenNone().
                            When(_ => _.Fail()).
                            Throws(FailNoExceptionCase.TheExpectedException).
                            Assert(new EqualsExceptionComparer()));
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
                public static readonly Exception TheExpectedException = new InvalidOperationException();
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

            class NullBuilder : IExceptionCentricAggregateCommandTestSpecificationBuilder
            {
                public static readonly IExceptionCentricAggregateCommandTestSpecificationBuilder Instance = new NullBuilder();

                public ExceptionCentricAggregateCommandTestSpecification Build()
                {
                    return null;
                }
            }
        }
    }
}
