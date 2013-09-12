using System;
using System.Linq;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    namespace FactoryScenarioForTests
    {
        [TestFixture]
        public class SutTests
        {
            [Test]
            public void SutIsSetInResultingSpecification()
            {
                var ctor = new AggregateRootEntityStub();

                var result = new FactoryScenarioFor<AggregateRootEntityStub>(ctor).
                    When(_ => new AggregateRootEntityStub()).
                    ThenNone().
                    Build().
                    SutFactory;

                Assert.That(result(), Is.SameAs(ctor));
            }
        }

        [TestFixture]
        public class SutFactoryTests
        {
            FactoryScenarioFor<AggregateRootEntityStub> _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new FactoryScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub());
            }

            [Test]
            public void FactoryCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => new FactoryScenarioFor<AggregateRootEntityStub>((Func<AggregateRootEntityStub>)null));
            }

            [Test]
            public void IsAggregateFactoryInitialStateBuilder()
            {
                Assert.IsInstanceOf<IAggregateFactoryInitialStateBuilder<AggregateRootEntityStub>>(_sut);
            }

            [Test]
            public void SutFactoryIsSetInResultingSpecification()
            {
                var ctor = new AggregateRootEntityStub();
                Func<AggregateRootEntityStub> factory = () => ctor;

                var result = new FactoryScenarioFor<AggregateRootEntityStub>(factory).
                    When(_ => new AggregateRootEntityStub()).
                    ThenNone().
                    Build().
                    SutFactory;

                Assert.That(result(), Is.SameAs(ctor));
            }
        }

        [TestFixture]
        public class FactoryScenarioForGivenTests : GivenFixture<AggregateRootEntityStub>
        {
            protected override IAggregateFactoryGivenStateBuilder<AggregateRootEntityStub> Given(params object[] events)
            {
                return new FactoryScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    Given(events);
            }
        }

        [TestFixture]
        public class AggregateFactoryGivenStateBuilderGivenTests : GivenFixture<AggregateRootEntityStub>
        {
            protected override IAggregateFactoryGivenStateBuilder<AggregateRootEntityStub> Given(params object[] events)
            {
                return new FactoryScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    Given(new object[0]).
                    Given(events);
            }
        }

        public abstract class GivenFixture<TAggregateRoot> where TAggregateRoot : IAggregateRootEntity
        {
            protected abstract IAggregateFactoryGivenStateBuilder<TAggregateRoot> Given(params object[] events);

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
                Assert.That(result, Is.InstanceOf<IAggregateFactoryGivenStateBuilder<TAggregateRoot>>());
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

                var result = Given(events).When(_ => new AggregateRootEntityStub()).ThenNone().Build().Givens;

                Assert.That(result, Is.EquivalentTo(events));
            }

            [Test]
            public void GivenEventsIsAppendOnly()
            {
                var events1 = new[] { new object(), new object() };
                var events2 = new[] { new object(), new object() };

                var result = Given(events1).Given(events2).When(_ => new AggregateRootEntityStub()).ThenNone().Build().Givens;

                Assert.That(result, Is.EquivalentTo(events1.Union(events2)));
            }
        }

        [TestFixture]
        public class FactoryScenarioForGivenNoneTests : GivenNoneFixture<AggregateRootEntityStub>
        {
            protected override IAggregateFactoryGivenNoneStateBuilder<AggregateRootEntityStub> GivenNone()
            {
                return new FactoryScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    GivenNone();
            }
        }

        public abstract class GivenNoneFixture<TAggregateRoot> where TAggregateRoot : IAggregateRootEntity
        {
            protected abstract IAggregateFactoryGivenNoneStateBuilder<TAggregateRoot> GivenNone();

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
                Assert.That(result, Is.InstanceOf<IAggregateFactoryGivenNoneStateBuilder<TAggregateRoot>>());
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
                var result = GivenNone().When(_ => new AggregateRootEntityStub()).ThenNone().Build().Givens;

                Assert.That(result, Is.Empty);
            }
        }

        [TestFixture]
        public class FactoryScenarioForWhenTests : WhenFixture<AggregateRootEntityStub, AggregateRootEntityStub>
        {
            protected override IAggregateFactoryWhenStateBuilder When(Func<AggregateRootEntityStub, AggregateRootEntityStub> query)
            {
                return new FactoryScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    When(query);
            }
        }

        [TestFixture]
        public class AggregateQueryGivenNoneStateBuilderWhenTests : WhenFixture<AggregateRootEntityStub, AggregateRootEntityStub>
        {
            protected override IAggregateFactoryWhenStateBuilder When(Func<AggregateRootEntityStub, AggregateRootEntityStub> query)
            {
                return new FactoryScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    GivenNone().
                    When(query);
            }
        }

        [TestFixture]
        public class AggregateQueryGivenStateBuilderWhenTests : WhenFixture<AggregateRootEntityStub, AggregateRootEntityStub>
        {
            protected override IAggregateFactoryWhenStateBuilder When(Func<AggregateRootEntityStub, AggregateRootEntityStub> query)
            {
                return new FactoryScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    Given(new object[0]).
                    When(query);
            }
        }

        public abstract class WhenFixture<TAggregateRoot, TAggregateRootResult>
            where TAggregateRoot : IAggregateRootEntity
            where TAggregateRootResult : IAggregateRootEntity
        {
            protected abstract IAggregateFactoryWhenStateBuilder When(Func<TAggregateRoot, TAggregateRootResult> query);

            [Test]
            public void WhenThrowsWhenQueryIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => When(null));
            }

            [Test]
            public void WhenDoesNotReturnNull()
            {
                var result = When(_ => default(TAggregateRootResult));
                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void WhenReturnsWhenBuilderContinuation()
            {
                var result = When(_ => default(TAggregateRootResult));
                Assert.That(result, Is.InstanceOf<IAggregateFactoryWhenStateBuilder>());
            }

            [Test]
            [Repeat(2)]
            public void WhenReturnsNewInstanceUponEachCall()
            {
                Assert.That(
                    When(_ => default(TAggregateRootResult)),
                    Is.Not.SameAs(When(_ => default(TAggregateRootResult))));
            }

            [Test]
            public void IsSetInResultingSpecification()
            {
                var called = false;
                Func<TAggregateRoot, TAggregateRootResult> query = _ =>
                {
                    called = true;
                    return default(TAggregateRootResult);
                };

                var specification = When(query).ThenNone().Build();

                var result = specification.When(specification.SutFactory());

                Assert.That(called, Is.True);
                Assert.That(result, Is.EqualTo(default(TAggregateRootResult)));
            }
        }


        [TestFixture]
        public class AggregateFactoryWhenStateBuilderThenTests : ThenFixture
        {
            protected override IAggregateFactoryThenStateBuilder Then(params object[] events)
            {
                return new FactoryScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    Given(new object[0]).
                    When(_ => new AggregateRootEntityStub()).
                    Then(events);
            }
        }

        [TestFixture]
        public class AggregateFactoryThenStateBuilderThenTests : ThenFixture
        {
            protected override IAggregateFactoryThenStateBuilder Then(params object[] events)
            {
                return new FactoryScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    Given(new object[0]).
                    When(_ => new AggregateRootEntityStub()).
                    Then(new object[0]).
                    Then(events);
            }
        }

        public abstract class ThenFixture
        {
            protected abstract IAggregateFactoryThenStateBuilder Then(params object[] events);

            [Test]
            public void ThenThrowsWhenEventsAreNull()
            {
                Assert.Throws<ArgumentNullException>(() => Then(null));
            }

            [Test]
            public void ThenDoesNotReturnNull()
            {
                var result = Then(new object[0]);
                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void ThenReturnsThenBuilderContinuation()
            {
                var result = Then(new object[0]);
                Assert.That(result, Is.InstanceOf<IAggregateFactoryThenStateBuilder>());
            }

            [Test]
            public void ThenReturnsNewInstanceUponEachCall()
            {
                Assert.That(
                    Then(new object[0]),
                    Is.Not.SameAs(Then(new object[0])));
            }

            [Test]
            public void ThenEventsAreSetInResultingSpecification()
            {
                var events = new[] { new object(), new object() };

                var result = Then(events).Build().Thens;

                Assert.That(result, Is.EquivalentTo(events));
            }

            [Test]
            public void ThenEventsIsAppendOnly()
            {
                var events1 = new[] { new object(), new object() };
                var events2 = new[] { new object(), new object() };

                var result = Then(events1).Then(events2).Build().Thens;

                Assert.That(result, Is.EquivalentTo(events1.Union(events2)));
            }
        }

        [TestFixture]
        public class AggregateFactoryWhenStateThenNoneTests : ThenNoneFixture
        {
            protected override IAggregateFactoryThenNoneStateBuilder ThenNone()
            {
                return new FactoryScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    Given(new object[0]).
                    When(_ => new AggregateRootEntityStub()).
                    ThenNone();
            }
        }

        public abstract class ThenNoneFixture
        {
            protected abstract IAggregateFactoryThenNoneStateBuilder ThenNone();

            [Test]
            public void ThenNoneDoesNotReturnNull()
            {
                var result = ThenNone();
                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void ThenNoneReturnsThenNoneBuilderContinuation()
            {
                var result = ThenNone();
                Assert.That(result, Is.InstanceOf<IAggregateFactoryThenNoneStateBuilder>());
            }

            [Test]
            public void ThenNoneReturnsNewInstanceUponEachCall()
            {
                Assert.That(
                    ThenNone(),
                    Is.Not.SameAs(ThenNone()));
            }

            [Test]
            public void ThenNoneEventsAreSetInResultingSpecification()
            {
                var result = ThenNone().Build().Thens;

                Assert.That(result, Is.Empty);
            }
        }
    }
}
