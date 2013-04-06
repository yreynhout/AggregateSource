using System;
using NUnit.Framework;

namespace AggregateSource.Testing {
  namespace ThenStateBuilderTests {
    [TestFixture]
    public class WhenBuilderThenTests : ThenFixture {
      protected override IThenStateBuilder Then(string identifier, params object[] events) {
        return new Scenario().When(new object()).Then(identifier, events);
      }
    }

    [TestFixture]
    public class ThenBuilderThenTests : ThenFixture {
      protected override IThenStateBuilder Then(string identifier, params object[] events) {
        return new Scenario().When(new object()).Then("", new object[0]).Then(identifier, events);
      }
    }

    public abstract class ThenFixture {
      protected abstract IThenStateBuilder Then(string identifier, params object[] events);

      [Test]
      public void ThenThrowsWhenIdentifierIsNull() {
        Assert.Throws<ArgumentNullException>(() => Then(null, new object[0]));
      }

      [Test]
      public void ThenThrowsWhenEventsAreNull() {
        Assert.Throws<ArgumentNullException>(() => Then(Model.Identifier, null));
      }

      [Test]
      public void ThenDoesNotReturnNull() {
        var result = Then(Model.Identifier, new object[0]);
        Assert.That(result, Is.Not.Null);
      }

      [Test]
      public void ThenReturnsThenBuilderContinuation() {
        var result = Then(Model.Identifier, new object[0]);
        Assert.That(result, Is.InstanceOf<IThenStateBuilder>());
      }

      [Test]
      [Repeat(2)]
      public void ThenReturnsNewInstanceUponEachCall() {
        Assert.That(
          Then(Model.Identifier, new object[0]),
          Is.Not.SameAs(Then(Model.Identifier, new object[0])));
      }

      [Test]
      public void IsSetInResultingSpecification() {
        var events = new[] {new object(), new object()};

        var result = Then(Model.Identifier, events).Build().Thens;

        Assert.That(result, Is.EquivalentTo(
          new[] {
            new Tuple<string, object>(Model.Identifier, events[0]),
            new Tuple<string, object>(Model.Identifier, events[1])
          }));
      }
    }
  }
}