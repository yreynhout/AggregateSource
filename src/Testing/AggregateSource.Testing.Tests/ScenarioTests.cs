using NUnit.Framework;

namespace AggregateSource.Testing
{
    namespace ScenarioTests
    {
        [TestFixture]
        public class ScenarioTests
        {
            [Test]
            public void IsScenarioInitialStateBuilder()
            {
                Assert.That(new Scenario(), Is.InstanceOf<IScenarioInitialStateBuilder>());
            }
        }

        [TestFixture]
        public class ScenarioGivenNoneTests : GivenNoneFixture
        {
            protected override IScenarioGivenNoneStateBuilder GivenNone()
            {
                return new Scenario().GivenNone();
            }
        }

        public abstract class GivenNoneFixture
        {
            protected abstract IScenarioGivenNoneStateBuilder GivenNone();

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
                Assert.That(result, Is.InstanceOf<IScenarioGivenNoneStateBuilder>());
            }

            [Test]
            [Repeat(2)]
            public void GivenNoneReturnsNewInstanceUponEachCall()
            {
                Assert.That(
                    GivenNone(),
                    Is.Not.SameAs(GivenNone()));
            }


            [Test]
            public void IsSetInResultingSpecification()
            {
                var result = GivenNone().When(new object()).Build().Givens;

                Assert.That(result, Is.EquivalentTo(Fact.Empty));
            }
        }
    }
}
