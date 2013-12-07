using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    namespace NUnitExtensionsForFactoryScenarioTests
    {
        [TestFixture]
        public class EventCentricAssert
        {
            [Test]
            public void BuilderCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(
                    () => ((IEventCentricAggregateFactoryTestSpecificationBuilder)null).Assert(null));
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
                        new FactoryScenarioFor<PassCase>(() => new PassCase()).
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
                        new FactoryScenarioFor<FailEventCountDifferenceCase>(() => new FailEventCountDifferenceCase()).
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
                        new FactoryScenarioFor<FailExceptionCase>(() => new FailExceptionCase()).
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
                        new FactoryScenarioFor<FailEventDifferenceCase>(() => new FailEventDifferenceCase()).
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

            class FailEventCountDifferenceCase : AggregateRootEntity
            {
                public static readonly object[] TheExpectedEvents = new object[0];

                public static readonly object[] TheActualEvents =
                {
                    new object()
                };

                public IAggregateRootEntity Fail()
                {
                    return new FactoryResult(TheActualEvents);
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

                public IAggregateRootEntity Fail()
                {
                    return new FactoryResult(TheActualEvents);
                }
            }

            class NullBuilder : IEventCentricAggregateFactoryTestSpecificationBuilder
            {
                public static readonly IEventCentricAggregateFactoryTestSpecificationBuilder Instance = new NullBuilder();

                public EventCentricAggregateFactoryTestSpecification Build()
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
                    () => ((IExceptionCentricAggregateFactoryTestSpecificationBuilder)null).Assert(null));
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
                        new FactoryScenarioFor<PassCase>(() => new PassCase()).
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
                        new FactoryScenarioFor<FailExceptionCase>(() => new FailExceptionCase()).
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
                        new FactoryScenarioFor<FailEventCase>(() => new FailEventCase()).
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
                        new FactoryScenarioFor<FailNoExceptionCase>(() => new FailNoExceptionCase()).
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
                public static readonly Exception TheException = new Exception();

                public IAggregateRootEntity Pass()
                {
                    throw TheException;
                }
            }

            class FailExceptionCase : AggregateRootEntity
            {
                public static readonly Exception TheExpectedException = new InvalidOperationException();
                public static readonly Exception TheActualException = new Exception();

                public IAggregateRootEntity Fail()
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

                public IAggregateRootEntity Fail()
                {
                    return new FactoryResult(TheEvents);
                }
            }

            class FailNoExceptionCase : AggregateRootEntity
            {
                public static readonly Exception TheExpectedException = new Exception();

                public IAggregateRootEntity Fail()
                {
                    return new FactoryResult(new object[0]);
                }
            }

            class NullBuilder : IExceptionCentricAggregateFactoryTestSpecificationBuilder
            {
                public static readonly IExceptionCentricAggregateFactoryTestSpecificationBuilder Instance = new NullBuilder();

                public ExceptionCentricAggregateFactoryTestSpecification Build()
                {
                    return null;
                }
            }
        }
    }
}
