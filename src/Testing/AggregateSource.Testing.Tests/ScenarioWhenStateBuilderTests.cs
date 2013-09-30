using System;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    namespace ScenarioWhenStateBuilderTests
    {
        [TestFixture]
        public class ScenarioWhenTests : WhenFixture
        {
            protected override IScenarioWhenStateBuilder When(object message)
            {
                return new Scenario().When(message);
            }
        }

        [TestFixture]
        public class GivenStateBuilderWhenTests : WhenFixture
        {
            protected override IScenarioWhenStateBuilder When(object message)
            {
                return new Scenario().Given("", new object[0]).When(message);
            }
        }

        public abstract class WhenFixture
        {
            protected abstract IScenarioWhenStateBuilder When(object message);

            [Test]
            public void WhenThrowsWhenMessageIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => When(null));
            }

            [Test]
            public void WhenDoesNotReturnNull()
            {
                var result = When(new object());
                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void WhenReturnsWhenBuilderContinuation()
            {
                var result = When(new object());
                Assert.That(result, Is.InstanceOf<IScenarioWhenStateBuilder>());
            }

            [Test]
            [Repeat(2)]
            public void WhenReturnsNewInstanceUponEachCall()
            {
                Assert.That(
                    When(new object()),
                    Is.Not.SameAs(When(new object())));
            }

            [Test]
            public void IsSetInResultingSpecification()
            {
                var message = new object();

                var result = When(message).Build().When;

                Assert.That(result, Is.SameAs(message));
            }
        }
    }
}