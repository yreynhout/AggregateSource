using System;
using System.Linq;
using NUnit.Framework;

namespace AggregateSource.Testing.AggregateBehavior
{
    namespace QueryScenarioForTests
    {
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
    }
}
