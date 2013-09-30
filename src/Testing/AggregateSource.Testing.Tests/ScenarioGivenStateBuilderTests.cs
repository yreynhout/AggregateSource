using System;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    namespace ScenarioGivenStateBuilderTests
    {
        [TestFixture]
        public class ScenarioGivenEventsTests : GivenEventsFixture
        {
            protected override IScenarioGivenStateBuilder Given(string identifier, params object[] events)
            {
                return new Scenario().Given(identifier, events);
            }
        }

        [TestFixture]
        public class GivenStateBuilderGivenEventsTests : GivenEventsFixture
        {
            protected override IScenarioGivenStateBuilder Given(string identifier, params object[] events)
            {
                return new Scenario().Given(Model.Identifier1, new object[0]).Given(identifier, events);
            }
        }

        public abstract class GivenEventsFixture
        {
            protected abstract IScenarioGivenStateBuilder Given(string identifier, params object[] events);

            [Test]
            public void GivenThrowsWhenIdentifierIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => Given(null, new object[0]));
            }

            [Test]
            public void GivenThrowsWhenEventsAreNull()
            {
                Assert.Throws<ArgumentNullException>(() => Given(Model.Identifier1, null));
            }

            [Test]
            public void GivenDoesNotReturnNull()
            {
                var result = Given(Model.Identifier1, new object[0]);
                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void GivenReturnsGivenBuilderContinuation()
            {
                var result = Given(Model.Identifier1, new object[0]);
                Assert.That(result, Is.InstanceOf<IScenarioGivenStateBuilder>());
            }

            [Test]
            [Repeat(2)]
            public void GivenReturnsNewInstanceUponEachCall()
            {
                Assert.That(
                    Given(Model.Identifier1, new object[0]),
                    Is.Not.SameAs(Given(Model.Identifier1, new object[0])));
            }

            [Test]
            public void IsSetInResultingSpecification()
            {
                var events = new[] {new object(), new object()};

                var result = Given(Model.Identifier1, events).When(new object()).Build().Givens;

                Assert.That(result, Is.EquivalentTo(
                    new[]
                    {
                        new Fact(Model.Identifier1, events[0]),
                        new Fact(Model.Identifier1, events[1])
                    }));
            }
        }

        [TestFixture]
        public class ScenarioGivenFactsTests : GivenFactsFixture
        {
            protected override IScenarioGivenStateBuilder Given(params Fact[] facts)
            {
                return new Scenario().Given(facts);
            }
        }

        [TestFixture]
        public class GivenStateBuilderGivenFactsTests : GivenFactsFixture
        {
            protected override IScenarioGivenStateBuilder Given(params Fact[] facts)
            {
                return new Scenario().Given(Model.Identifier1, new object[0]).Given(facts);
            }
        }

        public abstract class GivenFactsFixture
        {
            protected abstract IScenarioGivenStateBuilder Given(params Fact[] facts);

            Fact _fact;

            [SetUp]
            public void SetUp()
            {
                _fact = new Fact(Model.Identifier1, new object());
            }

            [Test]
            public void GivenThrowsWhenFactsAreNull()
            {
                Assert.Throws<ArgumentNullException>(() => Given(null));
            }

            [Test]
            public void GivenDoesNotReturnNull()
            {
                var result = Given(_fact);
                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void GivenReturnsGivenBuilderContinuation()
            {
                var result = Given(_fact);
                Assert.That(result, Is.InstanceOf<IScenarioGivenStateBuilder>());
            }

            [Test]
            [Repeat(2)]
            public void GivenReturnsNewInstanceUponEachCall()
            {
                Assert.That(
                    Given(_fact),
                    Is.Not.SameAs(Given(_fact)));
            }


            [Test]
            public void IsSetInResultingSpecification()
            {
                var facts = new[]
                {
                    new Fact(Model.Identifier1, new object()),
                    new Fact(Model.Identifier2, new object())
                };

                var result = Given(facts).When(new object()).Build().Givens;

                Assert.That(result, Is.EquivalentTo(facts));
            }
        }
    }
}