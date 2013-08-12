using System;
using NUnit.Framework;

namespace AggregateSource.Testing.CollaborationCentric
{
    namespace FactsBuilderTests
    {
        [TestFixture]
        public class InitialInstanceTests
        {
            FactsBuilder _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new FactsBuilder();
            }

            [Test]
            public void IsEmpty()
            {
                Fact[] result = _sut;

                Assert.That(result, Is.Empty);
            }
        }

        [TestFixture]
        public class StateFactTests : FactFixture {
            public override FactsBuilder Fact(string identifier, params object[] events)
            {
                return State.Fact(identifier, events);
            }
        }

        [TestFixture]
        public class FactsBuilderFactTests : FactFixture {
            public override FactsBuilder Fact(string identifier, params object[] events)
            {
                return new FactsBuilder().Fact(identifier, events);
            }
        }

        public abstract class FactFixture
        {
            public abstract FactsBuilder Fact(string identifier, params object[] events);

            [Test]
            public void IdentifierCannotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => Fact(null, new object[0]));
            }

            [Test]
            public void EventsCannotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => Fact(Model.Identifier1, null));
            }

            [Test]
            public void WhenNoEventsAreSpecifiedThenReturnsEmpty()
            {
                Fact[] result = Fact(Model.Identifier1, new object[0]);

                Assert.That(result, Is.Empty);
            }

            [Test]
            public void WhenEventsAreSpecifiedThenReturnsExpectedResult()
            {
                var event1 = new object();
                var event2 = new object();

                Fact[] result = Fact(Model.Identifier1, event1, event2);

                Assert.That(result, Is.EquivalentTo(
                    new[]
                    {
                        new Fact(Model.Identifier1, event1),
                        new Fact(Model.Identifier1, event2)
                    }));
            }

            [Test]
            public void WhenNewEventsAreSpecifiedThenReturnsCombinedExpectedResult()
            {
                var event1 = new object();
                var event2 = new object();

                var sut = Fact(Model.Identifier1, event1, event2);

                var event3 = new object();
                var event4 = new object();

                Fact[] result = sut.Fact(Model.Identifier2, event3, event4);

                Assert.That(result, Is.EquivalentTo(
                    new[]
                    {
                        new Fact(Model.Identifier1, event1),
                        new Fact(Model.Identifier1, event2),
                        new Fact(Model.Identifier2, event3),
                        new Fact(Model.Identifier2, event4)
                    }));
            }
        }

        [TestFixture]
        public class StateFactsTests : FactsFixture
        {
            public override FactsBuilder Facts(params Fact[] facts)
            {
                return State.Facts(facts);
            }
        }

        [TestFixture]
        public class FactsBuilderFactsTests : FactsFixture
        {
            public override FactsBuilder Facts(params Fact[] facts)
            {
                return new FactsBuilder().Facts(facts);
            }
        }

        public abstract class FactsFixture
        {
            public abstract FactsBuilder Facts(params Fact[] facts);

            [Test]
            public void FactsCannotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => Facts(null));
            }

            [Test]
            public void WhenNoFactsAreSpecifiedThenReturnsEmpty()
            {
                Fact[] result = Facts();

                Assert.That(result, Is.Empty);
            }

            [Test]
            public void WhenFactsAreSpecifiedThenReturnsExpectedResult()
            {
                var fact1 = new Fact(Model.Identifier1, new object());
                var fact2 = new Fact(Model.Identifier2, new object());

                Fact[] result = Facts(fact1, fact2);

                Assert.That(result, Is.EquivalentTo(
                    new[]
                    {
                        fact1,
                        fact2
                    }));
            }

            [Test]
            public void WhenNewEventsAreSpecifiedThenReturnsCombinedExpectedResult()
            {
                var fact1 = new Fact(Model.Identifier1, new object());
                var fact2 = new Fact(Model.Identifier2, new object());

                var sut = Facts(fact1, fact2);

                var fact3 = new Fact(Model.Identifier1, new object());
                var fact4 = new Fact(Model.Identifier2, new object());

                Fact[] result = sut.Facts(fact3, fact4);

                Assert.That(result, Is.EquivalentTo(new[] {fact1, fact2, fact3, fact4}));
            }
        }
    }
}