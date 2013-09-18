using System;
using System.Linq;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    namespace QueryScenarioForTests
    {
        [TestFixture]
        public class SutTests
        {
            [Test]
            public void SutIsSetInResultingSpecification()
            {
                var ctor = new AggregateRootEntityStub();

                var result = new QueryScenarioFor<AggregateRootEntityStub>(ctor).
                    When(_ => 0).
                    Then(0).
                    Build().
                    SutFactory;

                Assert.That(result(), Is.SameAs(ctor));
            }
        }

        [TestFixture]
        public class SutFactoryTests
        {
            QueryScenarioFor<AggregateRootEntityStub> _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new QueryScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub());
            }

            [Test]
            public void FactoryCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => new QueryScenarioFor<AggregateRootEntityStub>((Func<AggregateRootEntityStub>)null));
            }

            [Test]
            public void IsAggregateQueryInitialStateBuilder()
            {
                Assert.IsInstanceOf<IAggregateQueryInitialStateBuilder<AggregateRootEntityStub>>(_sut);
            }

            [Test]
            public void SutFactoryIsSetInResultingSpecification()
            {
                var ctor = new AggregateRootEntityStub();
                Func<AggregateRootEntityStub> factory = () => ctor;

                var result = new QueryScenarioFor<AggregateRootEntityStub>(factory).
                    When(_ => 0).
                    Then(0).
                    Build().
                    SutFactory;

                Assert.That(result(), Is.SameAs(ctor));
            }
        }

        [TestFixture]
        public class QueryScenarioForGivenTests : GivenFixture<AggregateRootEntityStub>
        {
            protected override IAggregateQueryGivenStateBuilder<AggregateRootEntityStub> Given(params object[] events)
            {
                return new QueryScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    Given(events);
            }
        }

        [TestFixture]
        public class AggregateQueryGivenStateBuilderGivenTests : GivenFixture<AggregateRootEntityStub>
        {
            protected override IAggregateQueryGivenStateBuilder<AggregateRootEntityStub> Given(params object[] events)
            {
                return new QueryScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    Given(new object[0]).
                    Given(events);
            }
        }

        public abstract class GivenFixture<TAggregateRoot> where TAggregateRoot : IAggregateRootEntity
        {
            protected abstract IAggregateQueryGivenStateBuilder<TAggregateRoot> Given(params object[] events);

            [Test]
            public void GivenThrowsWhenEventsAreNull()
            {
                Assert.Throws<ArgumentNullException>(() => Given(null));
            }

            [Test]
            public void GivenDoesNotReturnNull()
            {
                var result = Given(new object[0]);
                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void GivenReturnsGivenBuilderContinuation()
            {
                var result = Given(new object[0]);
                Assert.That(result, Is.InstanceOf<IAggregateQueryGivenStateBuilder<TAggregateRoot>>());
            }

            [Test]
            public void GivenReturnsNewInstanceUponEachCall()
            {
                Assert.That(
                    Given(new object[0]),
                    Is.Not.SameAs(Given(new object[0])));
            }

            [Test]
            public void GivenEventsAreSetInResultingSpecification()
            {
                var events = new[] { new object(), new object() };

                var result = Given(events).When(_ => 0).Then(0).Build().Givens;

                Assert.That(result, Is.EquivalentTo(events));
            }

            [Test]
            public void GivenEventsIsAppendOnly()
            {
                var events1 = new[] { new object(), new object() };
                var events2 = new[] { new object(), new object() };

                var result = Given(events1).Given(events2).When(_ => 0).Then(0).Build().Givens;

                Assert.That(result, Is.EquivalentTo(events1.Union(events2)));
            }
        }

        [TestFixture]
        public class QueryScenarioForGivenNoneTests : GivenNoneFixture<AggregateRootEntityStub>
        {
            protected override IAggregateQueryGivenNoneStateBuilder<AggregateRootEntityStub> GivenNone()
            {
                return new QueryScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    GivenNone();
            }
        }

        public abstract class GivenNoneFixture<TAggregateRoot> where TAggregateRoot : IAggregateRootEntity
        {
            protected abstract IAggregateQueryGivenNoneStateBuilder<TAggregateRoot> GivenNone();

            [Test]
            public void GivenNoneDoesNotReturnNull()
            {
                var result = GivenNone();
                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void GivenNoneReturnsGivenNoneBuilderContinuation()
            {
                var result = GivenNone();
                Assert.That(result, Is.InstanceOf<IAggregateQueryGivenNoneStateBuilder<TAggregateRoot>>());
            }

            [Test]
            public void GivenNoneReturnsNewInstanceUponEachCall()
            {
                Assert.That(
                    GivenNone(),
                    Is.Not.SameAs(GivenNone()));
            }

            [Test]
            public void GivenNoneEventsAreSetInResultingSpecification()
            {
                var result = GivenNone().When(_ => 0).Then(0).Build().Givens;

                Assert.That(result, Is.Empty);
            }
        }

        [TestFixture]
        public class QueryScenarioForWhenTests : WhenFixture<AggregateRootEntityStub, Int32> 
        {
            protected override IAggregateQueryWhenStateBuilder<int> When(Func<AggregateRootEntityStub, int> query)
            {
                return new QueryScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    When(query);
            }
        }

        [TestFixture]
        public class AggregateQueryGivenNoneStateBuilderWhenTests : WhenFixture<AggregateRootEntityStub, Int32>
        {
            protected override IAggregateQueryWhenStateBuilder<int> When(Func<AggregateRootEntityStub, int> query)
            {
                return new QueryScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    GivenNone().
                    When(query);
            }
        }

        [TestFixture]
        public class AggregateQueryGivenStateBuilderWhenTests : WhenFixture<AggregateRootEntityStub, Int32>
        {
            protected override IAggregateQueryWhenStateBuilder<int> When(Func<AggregateRootEntityStub, int> query)
            {
                return new QueryScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    Given(new object[0]).
                    When(query);
            }
        }

        public abstract class WhenFixture<TAggregateRoot, TResult> where TAggregateRoot : IAggregateRootEntity
        {
            protected abstract IAggregateQueryWhenStateBuilder<TResult> When(Func<TAggregateRoot, TResult> query);

            [Test]
            public void WhenThrowsWhenQueryIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => When(null));
            }

            [Test]
            public void WhenDoesNotReturnNull()
            {
                var result = When(_ => default(TResult));
                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void WhenReturnsWhenBuilderContinuation()
            {
                var result = When(_ => default(TResult));
                Assert.That(result, Is.InstanceOf<IAggregateQueryWhenStateBuilder<TResult>>());
            }

            [Test]
            [Repeat(2)]
            public void WhenReturnsNewInstanceUponEachCall()
            {
                Assert.That(
                    When(_ => default(TResult)),
                    Is.Not.SameAs(When(_ => default(TResult))));
            }

            [Test]
            public void IsSetInResultingSpecification()
            {
                var called = false;
                Func<TAggregateRoot, TResult> query = _ => { 
                    called = true;
                    return default(TResult);
                };

                var specification = When(query).Then(default(TResult)).Build();

                var result = specification.When(specification.SutFactory());

                Assert.That(called, Is.True);
                Assert.That(result, Is.EqualTo(default(TResult)));
            }
        }

        [TestFixture]
        public class AggregateQueryWhenStateBuilderThenTests : ThenFixture<Int32>{
            protected override IAggregateQueryThenStateBuilder Then(int result)
            {
                return new QueryScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    GivenNone().
                    When(_ => 0).
                    Then(result);
            }
        }

        public abstract class ThenFixture<TResult>
        {
            protected abstract IAggregateQueryThenStateBuilder Then(TResult result);

            [Test]
            public void ThenDoesNotReturnNull()
            {
                var result = Then(default(TResult));
                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void ThenReturnsWhenBuilderContinuation()
            {
                var result = Then(default(TResult));
                Assert.That(result, Is.InstanceOf<IAggregateQueryThenStateBuilder>());
            }

            [Test]
            [Repeat(2)]
            public void ThenReturnsNewInstanceUponEachCall()
            {
                Assert.That(
                    Then(default(TResult)),
                    Is.Not.SameAs(Then(default(TResult))));
            }

            [Test]
            public void IsSetInResultingSpecification()
            {
                var result = Then(default(TResult)).Build().Then;
                
                Assert.That(result, Is.EqualTo(default(TResult)));
            }
        }
    }
}
