using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    namespace NUnitExtensionsForQueryScenarioTests
    {
        [TestFixture]
        public class ResultCentricAssert
        {
            [Test]
            public void BuilderCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(
                    () => ((IResultCentricAggregateQueryTestSpecificationBuilder)null).Assert(null));
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
                        new QueryScenarioFor<PassCase>(() => new PassCase()).
                            GivenNone().
                            When(_ => _.Pass()).
                            Then(PassCase.TheResult).
                            Assert(new EqualsResultComparer()));
            }

            [Test]
            public void WhenSpecificationRunFailsWithResultDifference()
            {
                Assert.Throws<AssertionException>(
                    () =>
                        new QueryScenarioFor<FailResultCase>(() => new FailResultCase()).
                            GivenNone().
                            When(_ => _.Fail()).
                            Then(FailResultCase.TheExpectedResult).
                            Assert(new EqualsResultComparer()));
            }

            [Test]
            public void WhenSpecificationRunFailsWithException()
            {
                Assert.Throws<AssertionException>(
                    () =>
                        new QueryScenarioFor<FailExceptionCase>(() => new FailExceptionCase()).
                            GivenNone().
                            When(_ => _.Fail()).
                            Then(FailExceptionCase.TheResult).
                            Assert(new EqualsResultComparer()));
            }

            [Test]
            public void WhenSpecificationRunFailsWithEvents()
            {
                Assert.Throws<AssertionException>(
                    () =>
                        new QueryScenarioFor<FailEventCase>(() => new FailEventCase()).
                            GivenNone().
                            When(_ => _.Fail()).
                            Then(FailEventCase.TheResult).
                            Assert(new EqualsResultComparer()));
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

            class NullBuilder : IResultCentricAggregateQueryTestSpecificationBuilder
            {
                public static readonly IResultCentricAggregateQueryTestSpecificationBuilder Instance = new NullBuilder();

                public ResultCentricAggregateQueryTestSpecification Build()
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
                    () => ((IExceptionCentricAggregateQueryTestSpecificationBuilder)null).Assert(null));
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
                        new QueryScenarioFor<PassCase>(() => new PassCase()).
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
                        new QueryScenarioFor<FailExceptionCase>(() => new FailExceptionCase()).
                            GivenNone().
                            When(_ => _.Fail()).
                            Throws(FailExceptionCase.TheExpectedException).
                            Assert(new EqualsExceptionComparer()));
            }

            [Test]
            public void WhenSpecificationRunFailsBecauseEventsButNoException()
            {
                Assert.Throws<AssertionException>(
                    () =>
                        new QueryScenarioFor<FailEventButNoExceptionCase>(() => new FailEventButNoExceptionCase()).
                            GivenNone().
                            When(_ => _.Fail()).
                            Throws(FailEventButNoExceptionCase.TheExpectedException).
                            Assert(new EqualsExceptionComparer()));
            }

            [Test]
            public void WhenSpecificationRunFailsBecauseNoExceptionButResult()
            {
                Assert.Throws<AssertionException>(
                    () =>
                        new QueryScenarioFor<FailNoExceptionCase>(() => new FailNoExceptionCase()).
                            GivenNone().
                            When(_ => _.Fail()).
                            Throws(FailNoExceptionCase.TheExpectedException).
                            Assert(new EqualsExceptionComparer()));
            }

            [Test]
            public void WhenSpecificationRunFailsBecauseEventsButException()
            {
                Assert.Throws<AssertionException>(
                    () =>
                        new QueryScenarioFor<FailEventButExceptionCase>(() => new FailEventButExceptionCase()).
                            GivenNone().
                            When(_ => _.Fail()).
                            Throws(FailEventButExceptionCase.TheExpectedException).
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
                public static readonly int TheResult = 1;
                public static readonly Exception TheException = new Exception();

                public int Pass()
                {
                    throw TheException;
                }
            }

            class FailExceptionCase : AggregateRootEntity
            {
                public static readonly int TheResult = 1;
                public static readonly Exception TheExpectedException = new InvalidOperationException();
                public static readonly Exception TheActualException = new Exception();

                public int Fail()
                {
                    throw TheActualException;
                }
            }

            class FailEventButNoExceptionCase : AggregateRootEntity
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

            class FailEventButExceptionCase : AggregateRootEntity
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

                    throw TheExpectedException;
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

            class NullBuilder : IExceptionCentricAggregateQueryTestSpecificationBuilder
            {
                public static readonly IExceptionCentricAggregateQueryTestSpecificationBuilder Instance = new NullBuilder();

                public ExceptionCentricAggregateQueryTestSpecification Build()
                {
                    return null;
                }
            }
        }
    }
}
