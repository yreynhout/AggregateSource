using System;
using System.Linq;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    namespace CommandScenarioForTests
    {
        [TestFixture]
        public class SutTests
        {
            [Test]
            public void SutIsSetInResultingSpecification()
            {
                var ctor = new AggregateRootEntityStub();

                var result = new CommandScenarioFor<AggregateRootEntityStub>(ctor).
                    When(_ => { }).
                    Then(new object[0]).
                    Build().
                    SutFactory;

                Assert.That(result(), Is.SameAs(ctor));
            }
        }

        [TestFixture]
        public class SutFactoryTests
        {
            CommandScenarioFor<AggregateRootEntityStub> _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new CommandScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub());
            }

            [Test]
            public void FactoryCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => new CommandScenarioFor<AggregateRootEntityStub>((Func<AggregateRootEntityStub>)null));
            }

            [Test]
            public void IsAggregateCommandInitialStateBuilder()
            {
                Assert.IsInstanceOf<IAggregateCommandInitialStateBuilder<AggregateRootEntityStub>>(_sut);
            }

            [Test]
            public void SutFactoryIsSetInResultingSpecification()
            {
                var ctor = new AggregateRootEntityStub();
                Func<AggregateRootEntityStub> factory = () => ctor;

                var result = new CommandScenarioFor<AggregateRootEntityStub>(factory).
                    When(_ => { }).
                    Then(new object[0]).
                    Build().
                    SutFactory;

                Assert.That(result(), Is.SameAs(ctor));
            }
        }

        [TestFixture]
        public class CommandScenarioForGivenTests : GivenFixture<AggregateRootEntityStub>
        {
            protected override IAggregateCommandGivenStateBuilder<AggregateRootEntityStub> Given(params object[] events)
            {
                return new CommandScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    Given(events);
            }
        }

        [TestFixture]
        public class AggregateCommandGivenStateBuilderGivenTests : GivenFixture<AggregateRootEntityStub>
        {
            protected override IAggregateCommandGivenStateBuilder<AggregateRootEntityStub> Given(params object[] events)
            {
                return new CommandScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    Given(new object[0]).
                    Given(events);
            }
        }

        public abstract class GivenFixture<TAggregateRoot> where TAggregateRoot : IAggregateRootEntity
        {
            protected abstract IAggregateCommandGivenStateBuilder<TAggregateRoot> Given(params object[] events);

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
                Assert.That(result, Is.InstanceOf<IAggregateCommandGivenStateBuilder<TAggregateRoot>>());
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

                var result = Given(events).When(_ => { }).Then(new object[0]).Build().Givens;

                Assert.That(result, Is.EquivalentTo(events));
            }

            [Test]
            public void GivenEventsIsAppendOnly()
            {
                var events1 = new[] { new object(), new object() };
                var events2 = new[] { new object(), new object() };

                var result = Given(events1).Given(events2).When(_ => { }).Then(new object[0]).Build().Givens;

                Assert.That(result, Is.EquivalentTo(events1.Union(events2)));
            }
        }

        [TestFixture]
        public class CommandScenarioForGivenNoneTests : GivenNoneFixture<AggregateRootEntityStub>
        {
            protected override IAggregateCommandGivenNoneStateBuilder<AggregateRootEntityStub> GivenNone()
            {
                return new CommandScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    GivenNone();
            }
        }

        public abstract class GivenNoneFixture<TAggregateRoot> where TAggregateRoot : IAggregateRootEntity
        {
            protected abstract IAggregateCommandGivenNoneStateBuilder<TAggregateRoot> GivenNone();

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
                Assert.That(result, Is.InstanceOf<IAggregateCommandGivenNoneStateBuilder<TAggregateRoot>>());
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
                var result = GivenNone().When(_ => { }).Then(new object[0]).Build().Givens;

                Assert.That(result, Is.Empty);
            }
        }

        [TestFixture]
        public class CommandScenarioForWhenTests : WhenFixture<AggregateRootEntityStub>
        {
            protected override IAggregateCommandWhenStateBuilder When(Action<AggregateRootEntityStub> command)
            {
                return new CommandScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    When(command);
            }
        }

        [TestFixture]
        public class AggregateCommandGivenNoneStateBuilderWhenTests : WhenFixture<AggregateRootEntityStub>
        {
            protected override IAggregateCommandWhenStateBuilder When(Action<AggregateRootEntityStub> command)
            {
                return new CommandScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    GivenNone().
                    When(command);
            }
        }

        [TestFixture]
        public class AggregateCommandGivenStateBuilderWhenTests : WhenFixture<AggregateRootEntityStub>
        {
            protected override IAggregateCommandWhenStateBuilder When(Action<AggregateRootEntityStub> command)
            {
                return new CommandScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    Given(new object[0]).
                    When(command);
            }
        }

        public abstract class WhenFixture<TAggregateRoot> where TAggregateRoot : IAggregateRootEntity
        {
            protected abstract IAggregateCommandWhenStateBuilder When(Action<TAggregateRoot> command);

            [Test]
            public void WhenThrowsWhenCommandIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => When(null));
            }

            [Test]
            public void WhenDoesNotReturnNull()
            {
                var result = When(_ => {});
                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void WhenReturnsWhenBuilderContinuation()
            {
                var result = When(_ => { });
                Assert.That(result, Is.InstanceOf<IAggregateCommandWhenStateBuilder>());
            }

            [Test]
            [Repeat(2)]
            public void WhenReturnsNewInstanceUponEachCall()
            {
                Assert.That(
                    When(_ => { }),
                    Is.Not.SameAs(When(_ => { })));
            }

            [Test]
            public void IsSetInResultingSpecification()
            {
                var called = false;
                Action<TAggregateRoot> command = _ => { called = true; };

                var specification = When(command).ThenNone().Build();
                
                specification.When(specification.SutFactory());

                Assert.That(called, Is.True);
            }
        }

        [TestFixture]
        public class AggregateCommandWhenStateBuilderThenTests : ThenFixture
        {
            protected override IAggregateCommandThenStateBuilder Then(params object[] events)
            {
                return new CommandScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    Given(new object[0]).
                    When(_ => { }).
                    Then(events);
            }
        }

        [TestFixture]
        public class AggregateCommandThenStateBuilderThenTests : ThenFixture
        {
            protected override IAggregateCommandThenStateBuilder Then(params object[] events)
            {
                return new CommandScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    Given(new object[0]).
                    When(_ => { }).
                    Then(new object[0]).
                    Then(events);
            }
        }

        public abstract class ThenFixture
        {
            protected abstract IAggregateCommandThenStateBuilder Then(params object[] events);

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
                Assert.That(result, Is.InstanceOf<IAggregateCommandThenStateBuilder>());
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
        public class AggregateCommandWhenStateThenNoneTests : ThenNoneFixture
        {
            protected override IAggregateCommandThenNoneStateBuilder ThenNone()
            {
                return new CommandScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    Given(new object[0]).
                    When(_ => { }).
                    ThenNone();
            }
        }

        public abstract class ThenNoneFixture
        {
            protected abstract IAggregateCommandThenNoneStateBuilder ThenNone();

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
                Assert.That(result, Is.InstanceOf<IAggregateCommandThenNoneStateBuilder>());
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
