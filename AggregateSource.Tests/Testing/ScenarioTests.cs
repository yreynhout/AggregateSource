using System;
using NUnit.Framework;

namespace AggregateSource.Testing {
  namespace ScenarioTests {
    [TestFixture]
    public class GivenTests {
      [Test]
      public void GivenThrowsWhenEventsAreNull() {
        Assert.Throws<ArgumentNullException>(() => Scenario.Given(Guid.NewGuid(), null));
      }

      [Test]
      public void GivenDoesNotReturnNull() {
        var result = Scenario.Given(Guid.NewGuid(), new object[0]);
        Assert.That(result, Is.Not.Null);
      }

      [Test]
      public void GivenReturnsGivenBuilderContinuation() {
        var result = Scenario.Given(Guid.NewGuid(), new object[0]);
        Assert.That(result, Is.InstanceOf<IGivenStateBuilder>());
      }

      [Test]
      [Repeat(2)]
      public void GivenReturnsNewInstanceUponEachCall() {
        Assert.That(
          Scenario.Given(Guid.NewGuid(), new object[0]),
          Is.Not.SameAs(Scenario.Given(Guid.NewGuid(), new object[0])));
      }
    }

    [TestFixture]
    public class WhenTests {
      [Test]
      public void WhenThrowsWhenMessageIsNull() {
        Assert.Throws<ArgumentNullException>(() => Scenario.When(null));
      }

      [Test]
      public void WhenDoesNotReturnNull() {
        var result = Scenario.When(new object());
        Assert.That(result, Is.Not.Null);
      }

      [Test]
      public void WhenReturnsWhenBuilderContinuation() {
        var result = Scenario.When(new object());
        Assert.That(result, Is.InstanceOf<IWhenStateBuilder>());
      }

      [Test]
      [Repeat(2)]
      public void WhenReturnsNewInstanceUponEachCall() {
        Assert.That(
          Scenario.When(new object()),
          Is.Not.SameAs(Scenario.When(new object())));
      }
    }
  }
}
