using System;
using NUnit.Framework;

namespace AggregateSource.Testing {
  namespace GivenTests {
    [TestFixture]
    public class ScenarioGivenTests : GivenFixture {
      protected override IGivenStateBuilder Given(string identifier, params object[] events) {
        return new Scenario().Given(identifier, events);
      }
    }

    [TestFixture]
    public class GivenStateBuilderGivenTests : GivenFixture {
      protected override IGivenStateBuilder Given(string identifier, params object[] events) {
        return new Scenario().Given(Model.Identifier, new object[0]).Given(identifier, events);
      }
    }

    public abstract class GivenFixture {
      protected abstract IGivenStateBuilder Given(string identifier, params object[] events);

      [Test]
      public void GivenThrowsWhenIdentifierIsNull() {
        Assert.Throws<ArgumentNullException>(() => Given(null, new object[0]));
      }

      [Test]
      public void GivenThrowsWhenEventsAreNull() {
        Assert.Throws<ArgumentNullException>(() => Given(Model.Identifier, null));
      }

      [Test]
      public void GivenDoesNotReturnNull() {
        var result = Given(Model.Identifier, new object[0]);
        Assert.That(result, Is.Not.Null);
      }

      [Test]
      public void GivenReturnsGivenBuilderContinuation() {
        var result = Given(Model.Identifier, new object[0]);
        Assert.That(result, Is.InstanceOf<IGivenStateBuilder>());
      }

      [Test]
      [Repeat(2)]
      public void GivenReturnsNewInstanceUponEachCall() {
        Assert.That(
          Given(Model.Identifier, new object[0]),
          Is.Not.SameAs(Given(Model.Identifier, new object[0])));
      }

      [Test]
      public void IsSetInResultingSpecification() {
        var events = new[] { new object(), new object() };

        var result = Given(Model.Identifier, events).When(new object()).Build().Givens;

        Assert.That(result, Is.EquivalentTo(
          new[] {
            new Tuple<string, object>(Model.Identifier, events[0]),
            new Tuple<string, object>(Model.Identifier, events[1])
          }));
      }
    }
  }
}
