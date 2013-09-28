using System;
using System.Linq;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    namespace ConstructorScenarioForTests
    {
        [TestFixture]
        public class ConstructorTests
        {
            ConstructorScenarioFor<AggregateRootEntityStub> _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new ConstructorScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub());
            }

            [Test]
            public void ConstructorCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => new ConstructorScenarioFor<AggregateRootEntityStub>(null));
            }

            [Test]
            public void IsAggregateWhenStateBuilder()
            {
                Assert.IsInstanceOf<IAggregateConstructorWhenStateBuilder>(_sut);
            }

            [Test]
            public void ConstructorIsSetInResultingSpecification()
            {
                var ctor = new AggregateRootEntityStub();
                Func<AggregateRootEntityStub> factory = () => ctor;

                var result = new ConstructorScenarioFor<AggregateRootEntityStub>(factory).
                    Then(new object[0]).Build().SutFactory;

                Assert.That(result(), Is.SameAs(ctor));
            }
        }

        [TestFixture]
        public class ConstructorScenarioForThenTests : ThenFixture
        {
            protected override IAggregateConstructorThenStateBuilder Then(params object[] events)
            {
                return new ConstructorScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    Then(events);
            }
        }

        [TestFixture]
        public class AggregateConstructorThenStateThenTests : ThenFixture
        {
            protected override IAggregateConstructorThenStateBuilder Then(params object[] events)
            {
                return new ConstructorScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    Then(new object[0]).
                    Then(events);
            }
        }

        public abstract class ThenFixture
        {
            protected abstract IAggregateConstructorThenStateBuilder Then(params object[] events);

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
                Assert.That(result, Is.InstanceOf<IAggregateConstructorThenStateBuilder>());
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
                var events = new[] {new object(), new object()};

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
        public class ConstructorScenarioThrowsTests : ThrowsFixture
        {
            protected override IAggregateConstructorThrowStateBuilder Throws(Exception exception)
            {
                return new ConstructorScenarioFor<AggregateRootEntityStub>(() => new AggregateRootEntityStub()).
                    Throws(exception);
            }
        }

        public abstract class ThrowsFixture
        {
            protected abstract IAggregateConstructorThrowStateBuilder Throws(Exception exception);

            [Test]
            public void ThrowsThrowsWhenExceptionIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => Throws(null));
            }

            [Test]
            public void ThrowsDoesNotReturnNull()
            {
                var result = Throws(new Exception());

                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void ThrowsReturnsThrowBuilderContinuation()
            {
                var result = Throws(new Exception());

                Assert.That(result, Is.InstanceOf<IAggregateConstructorThrowStateBuilder>());
            }

            [Test]
            public void ThrowsReturnsNewInstanceUponEachCall()
            {
                Assert.That(
                    Throws(new Exception()),
                    Is.Not.SameAs(Throws(new Exception())));
            }

            [Test]
            public void ThrowsExceptionIsSetInResultingSpecification()
            {
                var exception = new Exception();

                var result = Throws(exception).Build().Throws;

                Assert.That(result, Is.SameAs(exception));
            }
        }
    }
}