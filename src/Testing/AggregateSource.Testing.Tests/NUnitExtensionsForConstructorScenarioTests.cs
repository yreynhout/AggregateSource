using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    namespace NUnitExtensionsForConstructorScenarioTests
    {
        [TestFixture]
        public class EventCentricAssert
        {
            [Test]
            public void BuilderCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(
                    () => ((IEventCentricAggregateConstructorTestSpecificationBuilder)null).Assert(null));
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
                        new ConstructorScenarioFor<PassCase>(() => new PassCase()).
                            Then(PassCase.TheExpectedEvents).
                            Assert(new EqualsEventComparer()));
            }

            [Test]
            public void WhenSpecificationRunFailsWithEventCountDifference()
            {
                Assert.Throws<AssertionException>(
                    () =>
                        new ConstructorScenarioFor<FailEventCountDifferenceCase>(() => new FailEventCountDifferenceCase()).
                            Then(FailEventCountDifferenceCase.TheExpectedEvents).
                            Assert(new EqualsEventComparer()));
            }

            [Test]
            public void WhenSpecificationRunFailsWithException()
            {
                Assert.Throws<AssertionException>(
                    () =>
                        new ConstructorScenarioFor<FailExceptionCase>(() => new FailExceptionCase()).
                            Then(FailExceptionCase.TheExpectedEvents).
                            Assert(new EqualsEventComparer()));
            }

            [Test]
            public void WhenSpecificationRunFailsWithEventDifferences()
            {
                Assert.Throws<AssertionException>(
                    () =>
                        new ConstructorScenarioFor<FailEventDifferenceCase>(() => new FailEventDifferenceCase()).
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

                public PassCase()
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

                public FailExceptionCase()
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

                public FailEventCountDifferenceCase()
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

                public FailEventDifferenceCase()
                {
                    foreach (var theEvent in TheActualEvents)
                    {
                        ApplyChange(theEvent);
                    }
                }
            }

            class NullBuilder : IEventCentricAggregateConstructorTestSpecificationBuilder
            {
                public static readonly IEventCentricAggregateConstructorTestSpecificationBuilder Instance = new NullBuilder();

                public EventCentricAggregateConstructorTestSpecification Build()
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
                    () => ((IExceptionCentricAggregateConstructorTestSpecificationBuilder)null).Assert(null));
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
                        new ConstructorScenarioFor<PassCase>(() => new PassCase()).
                        Throws(PassCase.TheException).
                        Assert(new EqualsExceptionComparer()));
            }

            [Test]
            public void WhenSpecificationRunFailsBecauseDifferentException()
            {
                Assert.Throws<AssertionException>(
                    () =>
                        new ConstructorScenarioFor<FailExceptionCase>(() => new FailExceptionCase()).
                            Throws(FailExceptionCase.TheExpectedException).
                            Assert(new EqualsExceptionComparer()));
            }

            [Test]
            public void WhenSpecificationRunFailsBecauseEvents()
            {
                Assert.Throws<AssertionException>(
                    () =>
                        new ConstructorScenarioFor<FailEventCase>(() => new FailEventCase()).
                            Throws(FailEventCase.TheExpectedException).
                            Assert(new EqualsExceptionComparer()));
            }

            [Test]
            public void WhenSpecificationRunFailsBecauseNoException()
            {
                Assert.Throws<AssertionException>(
                    () =>
                        new ConstructorScenarioFor<FailNoExceptionCase>(() => new FailNoExceptionCase()).
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

                public PassCase()
                {
                    throw TheException;
                }
            }

            class FailExceptionCase : AggregateRootEntity
            {
                public static readonly Exception TheExpectedException = new InvalidOperationException();
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

            class NullBuilder : IExceptionCentricAggregateConstructorTestSpecificationBuilder
            {
                public static readonly IExceptionCentricAggregateConstructorTestSpecificationBuilder Instance = new NullBuilder();

                public ExceptionCentricAggregateConstructorTestSpecification Build()
                {
                    return null;
                }
            }
        }
    }
}
