using System;
using AggregateSource.Testing.CollaborationCentric;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    namespace BatchOfTests
    {
        [TestFixture]
        public class InitialInstanceTests
        {
            State _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new State();
            }

            [Test]
            public void IsEmpty()
            {
                Tuple<string, object>[] result = _sut;

                Assert.That(result, Is.Empty);
            }
        }

        [TestFixture]
        public class FactTests
        {
            State _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new State();
            }

            [Test]
            public void IdentifierCannotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => _sut.Fact(null, new object[0]));
            }

            [Test]
            public void EventsCannotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => _sut.Fact(Model.Identifier1, null));
            }

            [Test]
            public void WhenNoEventsAreSpecifiedThenReturnsEmpty()
            {
                Tuple<string, object>[] result = _sut.Fact(Model.Identifier1, new object[0]);

                Assert.That(result, Is.Empty);
            }

            [Test]
            public void WhenEventsAreSpecifiedThenReturnsExpectedResult()
            {
                var event1 = new object();
                var event2 = new object();

                Tuple<string, object>[] result = _sut.Fact(Model.Identifier1, event1, event2);

                Assert.That(result, Is.EquivalentTo(
                    new[]
                    {
                        new Tuple<string, object>(Model.Identifier1, event1),
                        new Tuple<string, object>(Model.Identifier1, event2)
                    }));
            }

            [Test]
            public void WhenNewEventsAreSpecifiedThenReturnsCombinedExpectedResult()
            {
                var event1 = new object();
                var event2 = new object();

                var sut = _sut.Fact(Model.Identifier1, event1, event2);

                var event3 = new object();
                var event4 = new object();

                Tuple<string, object>[] result = sut.Fact(Model.Identifier2, event3, event4);

                Assert.That(result, Is.EquivalentTo(
                    new[]
                    {
                        new Tuple<string, object>(Model.Identifier1, event1),
                        new Tuple<string, object>(Model.Identifier1, event2),
                        new Tuple<string, object>(Model.Identifier2, event3),
                        new Tuple<string, object>(Model.Identifier2, event4)
                    }));
            }
        }


        [TestFixture]
        public class FactsTests
        {
            State _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new State();
            }

            [Test]
            public void FactsCannotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => _sut.Facts(null));
            }

            [Test]
            public void WhenNoFactsAreSpecifiedThenReturnsEmpty()
            {
                Tuple<string, object>[] result = _sut.Facts();

                Assert.That(result, Is.Empty);
            }

            [Test]
            public void WhenFactsAreSpecifiedThenReturnsExpectedResult()
            {
                var fact1 = new Tuple<string, object>(Model.Identifier1, new object());
                var fact2 = new Tuple<string, object>(Model.Identifier2, new object());

                Tuple<string, object>[] result = _sut.Facts(fact1, fact2);

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
                var fact1 = new Tuple<string, object>(Model.Identifier1, new object());
                var fact2 = new Tuple<string, object>(Model.Identifier2, new object());

                var sut = _sut.Facts(fact1, fact2);

                var fact3 = new Tuple<string, object>(Model.Identifier1, new object());
                var fact4 = new Tuple<string, object>(Model.Identifier2, new object());

                Tuple<string, object>[] result = sut.Facts(fact3, fact4);

                Assert.That(result, Is.EquivalentTo(new[] {fact1, fact2, fact3, fact4}));
            }
        }
    }
}