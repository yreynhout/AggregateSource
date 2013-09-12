using System;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    namespace ScenarioThenStateBuilderTests
    {
        [TestFixture]
        public class WhenBuilderThenEventsTests : ThenEventsFixture
        {
            protected override IScenarioThenStateBuilder Then(string identifier, params object[] events)
            {
                return new Scenario().When(new object()).Then(identifier, events);
            }
        }

        [TestFixture]
        public class ThenEventsBuilderThenEventsTests : ThenEventsFixture
        {
            protected override IScenarioThenStateBuilder Then(string identifier, params object[] events)
            {
                return new Scenario().When(new object()).Then("", new object[0]).Then(identifier, events);
            }
        }

        public abstract class ThenEventsFixture
        {
            protected abstract IScenarioThenStateBuilder Then(string identifier, params object[] events);

            [Test]
            public void ThenThrowsWhenIdentifierIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => Then(null, new object[0]));
            }

            [Test]
            public void ThenThrowsWhenEventsAreNull()
            {
                Assert.Throws<ArgumentNullException>(() => Then(Model.Identifier1, null));
            }

            [Test]
            public void ThenDoesNotReturnNull()
            {
                var result = Then(Model.Identifier1, new object[0]);
                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void ThenReturnsThenBuilderContinuation()
            {
                var result = Then(Model.Identifier1, new object[0]);
                Assert.That(result, Is.InstanceOf<IScenarioThenStateBuilder>());
            }

            [Test]
            [Repeat(2)]
            public void ThenReturnsNewInstanceUponEachCall()
            {
                Assert.That(
                    Then(Model.Identifier1, new object[0]),
                    Is.Not.SameAs(Then(Model.Identifier1, new object[0])));
            }

            [Test]
            public void IsSetInResultingSpecification()
            {
                var events = new[] {new object(), new object()};

                var result = Then(Model.Identifier1, events).Build().Thens;

                Assert.That(result, Is.EquivalentTo(
                    new[]
                    {
                        new Fact(Model.Identifier1, events[0]),
                        new Fact(Model.Identifier1, events[1])
                    }));
            }
        }

        [TestFixture]
        public class WhenBuilderThenFactsTests : ThenFactsFixture
        {
            protected override IScenarioThenStateBuilder Then(params Fact[] facts)
            {
                return new Scenario().When(new object()).Then(facts);
            }
        }

        [TestFixture]
        public class ThenFactsBuilderThenFactsTests : ThenFactsFixture
        {
            protected override IScenarioThenStateBuilder Then(params Fact[] facts)
            {
                return new Scenario().When(new object()).Then("", new object[0]).Then(facts);
            }
        }

        public abstract class ThenFactsFixture
        {
            protected abstract IScenarioThenStateBuilder Then(params Fact[] facts);

            Fact _fact;

            [SetUp]
            public void SetUp()
            {
                _fact = new Fact(Model.Identifier1, new object());
            }

            [Test]
            public void ThenThrowsWhenFactsAreNull()
            {
                Assert.Throws<ArgumentNullException>(() => Then(null));
            }

            [Test]
            public void ThenDoesNotReturnNull()
            {
                var result = Then(_fact);
                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void ThenReturnsThenBuilderContinuation()
            {
                var result = Then(_fact);
                Assert.That(result, Is.InstanceOf<IScenarioThenStateBuilder>());
            }

            [Test]
            [Repeat(2)]
            public void ThenReturnsNewInstanceUponEachCall()
            {
                Assert.That(
                    Then(_fact),
                    Is.Not.SameAs(Then(_fact)));
            }

            [Test]
            public void IsSetInResultingSpecification()
            {
                var facts = new[]
                {
                    new Fact(Model.Identifier1, new object()),
                    new Fact(Model.Identifier2, new object())
                };

                var result = Then(facts).Build().Thens;

                Assert.That(result, Is.EquivalentTo(facts));
            }
        }

        [TestFixture]
        public class WhenBuilderThenNoneTests : ThenNoneFixture
        {
            protected override IScenarioThenNoneStateBuilder ThenNone()
            {
                return new Scenario().When(new object()).ThenNone();
            }
        }

        [TestFixture]
        public class ThenEventsBuilderThenNoneTests : ThenNoneFixture
        {
            protected override IScenarioThenNoneStateBuilder ThenNone()
            {
                return new Scenario().When(new object()).ThenNone();
            }
        }

        public abstract class ThenNoneFixture
        {
            protected abstract IScenarioThenNoneStateBuilder ThenNone();

            [Test]
            public void ThenNoneDoesNotReturnNull()
            {
                var result = ThenNone();
                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void ThenNoneReturnsThenBuilderContinuation()
            {
                var result = ThenNone();
                Assert.That(result, Is.InstanceOf<IScenarioThenNoneStateBuilder>());
            }

            [Test]
            [Repeat(2)]
            public void ThenReturnsNewInstanceUponEachCall()
            {
                Assert.That(
                    ThenNone(),
                    Is.Not.SameAs(ThenNone()));
            }

            [Test]
            public void IsSetInResultingSpecification()
            {
                var result = ThenNone().Build().Thens;

                Assert.That(result, Is.EquivalentTo(Fact.Empty));
            }
        }
    }
}