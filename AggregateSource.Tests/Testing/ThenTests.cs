using System;
using NUnit.Framework;

namespace AggregateSource.Testing {
  namespace ThenTests {
    [TestFixture]
    public class WhenBuilderThenTests : ThenFixture {
      protected override IThenStateBuilder Then(Guid id, params object[] events) {
        return Scenario.When(new object()).Then(id, events);
      }
    }

    [TestFixture]
    public class ThenBuilderThenTests : ThenFixture {
      protected override IThenStateBuilder Then(Guid id, params object[] events) {
        return Scenario.When(new object()).Then(Guid.Empty, new object[0]).Then(id, events);
      }
    }

    public abstract class ThenFixture {
      protected abstract IThenStateBuilder Then(Guid id, params object[] events);

      [Test]
      public void ThenThrowsWhenEventsAreNull() {
        Assert.Throws<ArgumentNullException>(() => Then(Guid.NewGuid(), null));
      }

      [Test]
      public void ThenDoesNotReturnNull() {
        var result = Then(Guid.NewGuid(), new object[0]);
        Assert.That(result, Is.Not.Null);
      }

      [Test]
      public void ThenReturnsThenBuilderContinuation() {
        var result = Then(Guid.NewGuid(), new object[0]);
        Assert.That(result, Is.InstanceOf<IThenStateBuilder>());
      }

      [Test]
      [Repeat(2)]
      public void ThenReturnsNewInstanceUponEachCall() {
        Assert.That(
          Then(Guid.NewGuid(), new object[0]),
          Is.Not.SameAs(Then(Guid.NewGuid(), new object[0])));
      }

      [Test]
      public void IsSetInResultingSpecification() {
        var id = Guid.NewGuid();
        var events = new[] {new object(), new object()};

        var result = Then(id, events).Build().Thens;

        Assert.That(result, Is.EquivalentTo(
          new[] {
            new Tuple<Guid, object>(id, events[0]),
            new Tuple<Guid, object>(id, events[1])
          }));
      }
    }
  }
}