using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource.Testing.AggregateBehavior
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
                    () => NUnitExtensionsForFactoryScenario.Assert((IEventCentricAggregateFactoryTestSpecificationBuilder)null, null));
            }

            [Test]
            public void ComparerCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(
                    () => NUnitExtensionsForFactoryScenario.Assert(BuilderStub.Instance, null));
            }

            [Test]
            public void WhenSpecificationRunPasses()
            {
                Assert.DoesNotThrow(
                    () => NUnitExtensionsForFactoryScenario.Assert(SuccessBuilderStub.Instance, ReferenceEqualityEventComparer.Instance));
            }

            [Test]
            public void WhenSpecificationRunFailsWithEventCountDifference()
            {
                Assert.Throws<AssertionException>(
                    () => NUnitExtensionsForFactoryScenario.Assert(FailureWithEventCountDifferenceBuilderStub.Instance, ReferenceEqualityEventComparer.Instance));
            }

            [Test]
            public void WhenSpecificationRunFailsWithException()
            {
                Assert.Throws<AssertionException>(
                    () => NUnitExtensionsForFactoryScenario.Assert(FailureWithExceptionBuilderStub.Instance, ReferenceEqualityEventComparer.Instance));
            }

            [Test]
            public void WhenSpecificationRunFailsWithEventDifferences()
            {
                Assert.Throws<AssertionException>(
                    () => NUnitExtensionsForFactoryScenario.Assert(FailureWithEventDifferencesBuilderStub.Instance, ReferenceEqualityEventComparer.Instance));
            }

            class BuilderStub : IEventCentricAggregateFactoryTestSpecificationBuilder
            {
                public static readonly IEventCentricAggregateFactoryTestSpecificationBuilder Instance = new BuilderStub();

                public EventCentricAggregateFactoryTestSpecification Build()
                {
                    return null;
                }
            }

            class SuccessBuilderStub : IEventCentricAggregateFactoryTestSpecificationBuilder
            {
                public static readonly IEventCentricAggregateFactoryTestSpecificationBuilder Instance = new SuccessBuilderStub();

                public EventCentricAggregateFactoryTestSpecification Build()
                {
                    return new EventCentricAggregateFactoryTestSpecification(
                        () => new LocalAggregateRootEntityStub(),
                        new object[0],
                        _ => ((LocalAggregateRootEntityStub)_).SuccessFactory(),
                        new[] { LocalAggregateRootEntityStub.Event });
                }
            }

            class FailureWithEventCountDifferenceBuilderStub : IEventCentricAggregateFactoryTestSpecificationBuilder
            {
                public static readonly IEventCentricAggregateFactoryTestSpecificationBuilder Instance = new FailureWithEventCountDifferenceBuilderStub();

                public EventCentricAggregateFactoryTestSpecification Build()
                {
                    return new EventCentricAggregateFactoryTestSpecification(
                        () => new LocalAggregateRootEntityStub(),
                        new object[0],
                        _ => ((LocalAggregateRootEntityStub)_).EventCountDifferenceFailureFactory(),
                        new[] { LocalAggregateRootEntityStub.Event });
                }
            }

            class FailureWithEventDifferencesBuilderStub : IEventCentricAggregateFactoryTestSpecificationBuilder
            {
                public static readonly IEventCentricAggregateFactoryTestSpecificationBuilder Instance = new FailureWithEventDifferencesBuilderStub();

                public EventCentricAggregateFactoryTestSpecification Build()
                {
                    return new EventCentricAggregateFactoryTestSpecification(
                        () => new LocalAggregateRootEntityStub(),
                        new object[0],
                        _ => ((LocalAggregateRootEntityStub)_).EventDifferencesFailureFactory(),
                        new[] { LocalAggregateRootEntityStub.Event });
                }
            }

            class FailureWithExceptionBuilderStub : IEventCentricAggregateFactoryTestSpecificationBuilder
            {
                public static readonly IEventCentricAggregateFactoryTestSpecificationBuilder Instance = new FailureWithExceptionBuilderStub();

                public EventCentricAggregateFactoryTestSpecification Build()
                {
                    return new EventCentricAggregateFactoryTestSpecification(
                        () => new LocalAggregateRootEntityStub(),
                        new object[0],
                        _ => ((LocalAggregateRootEntityStub)_).ExceptionFailureFactory(),
                        new[] { LocalAggregateRootEntityStub.Event });
                }
            }

            class LocalAggregateRootEntityStub : AggregateRootEntity
            {
                public static readonly object Event = new object();
                public static readonly Exception Exception = new Exception();

                public LocalAggregateRootEntityStub()
                {
                }

                LocalAggregateRootEntityStub(object @event)
                {
                    Apply(@event);
                }

                public LocalAggregateRootEntityStub SuccessFactory()
                {
                    return new LocalAggregateRootEntityStub(Event);
                }

                public LocalAggregateRootEntityStub EventCountDifferenceFailureFactory()
                {
                    return new LocalAggregateRootEntityStub();
                }

                public LocalAggregateRootEntityStub EventDifferencesFailureFactory()
                {
                    return new LocalAggregateRootEntityStub(new object());
                }

                public LocalAggregateRootEntityStub ExceptionFailureFactory()
                {
                    throw Exception;
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
                    () => NUnitExtensionsForFactoryScenario.Assert((IExceptionCentricAggregateFactoryTestSpecificationBuilder)null, null));
            }

            [Test]
            public void ComparerCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(
                    () => NUnitExtensionsForFactoryScenario.Assert(BuilderStub.Instance, null));
            }

            class BuilderStub : IExceptionCentricAggregateFactoryTestSpecificationBuilder
            {
                public static readonly IExceptionCentricAggregateFactoryTestSpecificationBuilder Instance = new BuilderStub();

                public ExceptionCentricAggregateFactoryTestSpecification Build()
                {
                    return null;
                }
            }
        }

        class ReferenceEqualityEventComparer : IEventComparer
        {
            public static readonly IEventComparer Instance = new ReferenceEqualityEventComparer();

            public IEnumerable<EventComparisonDifference> Compare(object expected, object actual)
            {
                if (!ReferenceEquals(expected, actual))
                    yield return new EventComparisonDifference(
                        expected,
                        actual,
                        "Failure");
            }
        }
    }
}
