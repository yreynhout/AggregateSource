using System;
using NUnit.Framework;

namespace AggregateSource.Testing {
  namespace GivenTests {
    [TestFixture]
    public class ScenarioGivenTests : GivenFixture {
      protected override IGivenStateBuilder Given(Guid id, params object[] events) {
        return new Scenario().Given(id, events);
      }
    }

    [TestFixture]
    public class GivenStateBuilderGivenTests : GivenFixture {
      protected override IGivenStateBuilder Given(Guid id, params object[] events) {
        return new Scenario().Given(Guid.Empty, new object[0]).Given(id, events);
      }
    }

    public abstract class GivenFixture {
      protected abstract IGivenStateBuilder Given(Guid id, params object[] events);

      [Test]
      public void GivenThrowsWhenEventsAreNull() {
        Assert.Throws<ArgumentNullException>(() => Given(Guid.NewGuid(), null));
      }

      [Test]
      public void GivenDoesNotReturnNull() {
        var result = Given(Guid.NewGuid(), new object[0]);
        Assert.That(result, Is.Not.Null);
      }

      [Test]
      public void GivenReturnsGivenBuilderContinuation() {
        var result = Given(Guid.NewGuid(), new object[0]);
        Assert.That(result, Is.InstanceOf<IGivenStateBuilder>());
      }

      [Test]
      [Repeat(2)]
      public void GivenReturnsNewInstanceUponEachCall() {
        Assert.That(
          Given(Guid.NewGuid(), new object[0]),
          Is.Not.SameAs(Given(Guid.NewGuid(), new object[0])));
      }

      [Test]
      public void IsSetInResultingSpecification() {
        var id = Guid.NewGuid();
        var events = new[] { new object(), new object() };

        var result = Given(id, events).When(new object()).Build().Givens;

        Assert.That(result, Is.EquivalentTo(
          new[] {
            new Tuple<Guid, object>(id, events[0]),
            new Tuple<Guid, object>(id, events[1])
          }));
      }
    }
  }
}
