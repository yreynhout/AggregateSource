using System;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    namespace ScenarioThrowStateBuilderTests
    {
        [TestFixture]
        public class WhenStateBuilderThrowTests : ThrowFixture
        {
            protected override IScenarioThrowStateBuilder Throw(Exception exception)
            {
                return new Scenario().Given("", new object[0]).When(new object()).Throws(exception);
            }
        }

        public abstract class ThrowFixture
        {
            protected abstract IScenarioThrowStateBuilder Throw(Exception exception);

            [Test]
            public void ThrowThrowsWhenExceptionIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => Throw(null));
            }

            [Test]
            public void ThrowDoesNotReturnNull()
            {
                var result = Throw(new Exception());
                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void ThrowReturnsThrowBuilderContinuation()
            {
                var result = Throw(new Exception());
                Assert.That(result, Is.InstanceOf<IScenarioThrowStateBuilder>());
            }

            [Test]
            [Repeat(2)]
            public void ThrowReturnsNewInstanceUponEachCall()
            {
                Assert.That(
                    Throw(new Exception()),
                    Is.Not.SameAs(Throw(new Exception())));
            }

            [Test]
            public void IsSetInResultingSpecification()
            {
                var exception = new Exception();

                var result = Throw(exception).Build().Throws;

                Assert.That(result, Is.SameAs(exception));
            }
        }
    }
}