using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AggregateSource.Tests {
  namespace AggregateRootEntityTests {
    [TestFixture]
    public class WithPristineInstance {
      AggregateRootEntity _sut;

      [SetUp]
      public void SetUp() {
        _sut = new PristineAggregateRootEntity();
      }

      [Test]
      public void ClearChangesDoesNothing() {
        Assert.DoesNotThrow(() => _sut.ClearChanges());
      }

      [Test]
      public void HasChangesReturnsFalse() {
        Assert.That(_sut.HasChanges(), Is.False);
      }

      [Test]
      public void GetChangesReturnsEmpty() {
        Assert.That(_sut.GetChanges(), Is.EquivalentTo(Enumerable.Empty<object>()));
      }
    }

    class PristineAggregateRootEntity : AggregateRootEntity { }

    [TestFixture]
    public class WithInitializedInstance {
      AggregateRootEntity _sut;

      [SetUp]
      public void SetUp() {
        _sut = new InitializedAggregateRootEntity();
      }

      [Test]
      public void ClearChangesDoesNothing() {
        Assert.DoesNotThrow(() => _sut.ClearChanges());
      }

      [Test]
      public void HasChangesReturnsFalse() {
        Assert.That(_sut.HasChanges(), Is.False);
      }

      [Test]
      public void GetChangesReturnsEmpty() {
        Assert.That(_sut.GetChanges(), Is.EquivalentTo(Enumerable.Empty<object>()));
      }
    }
    
    class InitializedAggregateRootEntity : AggregateRootEntity {
      public InitializedAggregateRootEntity() {
        Initialize(new [] { new object(), new object() });
      }
    }

    [TestFixture]
    public class WithChangedInstance {
      AggregateRootEntity _sut;

      [SetUp]
      public void SetUp() {
        _sut = new ChangedAggregateRootEntity();
      }

      [Test]
      public void ClearChangesDoesNothing() {
        Assert.DoesNotThrow(() => _sut.ClearChanges());
      }

      [Test]
      public void HasChangesReturnsFalse() {
        Assert.That(_sut.HasChanges(), Is.True);
      }

      [Test]
      public void GetChangesReturnsEmpty() {
        Assert.That(_sut.GetChanges(), Is.EquivalentTo(ChangedAggregateRootEntity.AppliedChanges));
      }
    }

    class ChangedAggregateRootEntity : AggregateRootEntity {
      public static readonly object[] AppliedChanges = new [] { new object(), new object() };
      public ChangedAggregateRootEntity() {
        foreach (var change in AppliedChanges) {
          Apply(change);  
        }
      }
    }

    [TestFixture]
    public class WithFirstChangedThenClearedInstance {
      AggregateRootEntity _sut;

      [SetUp]
      public void SetUp() {
        _sut = new FirstChangedThenClearedAggregateRootEntity();
      }

      [Test]
      public void ClearChangesDoesNothing() {
        Assert.DoesNotThrow(() => _sut.ClearChanges());
      }

      [Test]
      public void HasChangesReturnsFalse() {
        Assert.That(_sut.HasChanges(), Is.False);
      }

      [Test]
      public void GetChangesReturnsEmpty() {
        Assert.That(_sut.GetChanges(), Is.EquivalentTo(Enumerable.Empty<object>()));
      }
    }

    class FirstChangedThenClearedAggregateRootEntity : AggregateRootEntity {
      public FirstChangedThenClearedAggregateRootEntity() {
        foreach (var change in new[] { new object(), new object() }) {
          Apply(change);
        }
        ClearChanges();
      }
    }

    [TestFixture]
    public class WithFirstChangedThenInitializedInstance {
      AggregateRootEntity _sut;

      [SetUp]
      public void SetUp() {
        _sut = new FirstChangedThenInitializedAggregateRootEntity();
      }

      [Test]
      public void ClearChangesDoesNothing() {
        Assert.DoesNotThrow(() => _sut.ClearChanges());
      }

      [Test]
      public void HasChangesReturnsFalse() {
        Assert.That(_sut.HasChanges(), Is.False);
      }

      [Test]
      public void GetChangesReturnsEmpty() {
        Assert.That(_sut.GetChanges(), Is.EquivalentTo(Enumerable.Empty<object>()));
      }
    }

    class FirstChangedThenInitializedAggregateRootEntity : AggregateRootEntity {
      public FirstChangedThenInitializedAggregateRootEntity() {
        foreach (var change in new[] { new object(), new object() }) {
          Apply(change);
        }
        Initialize(new[] { new object(), new object() });
      }
    }

    [TestFixture]
    public class WithInstanceWithHandlers {
      [Test]
      public void InitializeCallsHandlerForEachEvent() {
        var sut = new InitializedWithHandlerExpectedCallCountAggregateRootEntity(2);
        Assert.That(sut.HandlerActualCallCount, Is.EqualTo(2));
      }

      [Test]
      public void InitializeCallsHandlerWithExpectedEvent() {
        var expectedEvents = new [] {new object(), new object()};
        var sut = new InitializedWithExpectedEventsAggregateRootEntity(expectedEvents);
        Assert.That(sut.ActualEvents, Is.EquivalentTo(expectedEvents));
      }
    }

    class InitializedWithHandlerExpectedCallCountAggregateRootEntity : AggregateRootEntity {
      public int HandlerActualCallCount { get; private set; }

      public InitializedWithHandlerExpectedCallCountAggregateRootEntity(int handlerExpectedCallCount) {
        HandlerActualCallCount = 0;
        Register<object>(@event => { HandlerActualCallCount++; });
        var events = new object[handlerExpectedCallCount];
        for (var index = 0; index < handlerExpectedCallCount; index++) {
          events[index] = new object();
        }
        Initialize(events);
      }
    }
    class InitializedWithExpectedEventsAggregateRootEntity : AggregateRootEntity {
      public List<object> ActualEvents { get; private set; }

      public InitializedWithExpectedEventsAggregateRootEntity(IEnumerable<object> expectedEvents) {
        ActualEvents = new List<object>();
        Register<object>(@event => ActualEvents.Add(@event));
        Initialize(expectedEvents);
      }
    }

    [TestFixture]
    public class WithChangedInstanceWithHandlers {
      [Test]
      public void ApplyEventCallsHandlerForEachEvent() {
        var sut = new ChangedWithHandlerExpectedCallCountAggregateRootEntity(2);
        Assert.That(sut.HandlerActualCallCount, Is.EqualTo(2));
      }

      [Test]
      public void ApplyEventCallsHandlerWithExpectedEvent() {
        var expectedEvents = new[] { new object(), new object() };
        var sut = new ChangedWithExpectedEventsAggregateRootEntity(expectedEvents);
        Assert.That(sut.ActualEvents, Is.EquivalentTo(expectedEvents));
      }
    }

    class ChangedWithHandlerExpectedCallCountAggregateRootEntity : AggregateRootEntity {
      public int HandlerActualCallCount { get; private set; }

      public ChangedWithHandlerExpectedCallCountAggregateRootEntity(int handlerExpectedCallCount) {
        HandlerActualCallCount = 0;
        Register<object>(@event => { HandlerActualCallCount++; });
        for (var index = 0; index < handlerExpectedCallCount; index++) {
          Apply(new object());
        }
      }
    }
    class ChangedWithExpectedEventsAggregateRootEntity : AggregateRootEntity {
      public List<object> ActualEvents { get; private set; }

      public ChangedWithExpectedEventsAggregateRootEntity(IEnumerable<object> expectedEvents) {
        ActualEvents = new List<object>();
        Register<object>(@event => ActualEvents.Add(@event));
        foreach (var @event in expectedEvents) {
          Apply(@event);
        }
      }
    }

    [TestFixture]
    public class WithInstanceWithNullHandler {
      [Test]
      public void HandlerCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new NullHandlerAggregateRootEntity());
      }
    }

    class NullHandlerAggregateRootEntity : AggregateRootEntity {
      public NullHandlerAggregateRootEntity() {
        Register<object>(null);
      }
    }

    [TestFixture]
    public class WithInstanceInitializedWithNullEvents {
      [Test]
      public void InitializeEventsCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new IntializedWithNullEventsAggregateRootEntity());
      }
    }

    class IntializedWithNullEventsAggregateRootEntity : AggregateRootEntity {
      public IntializedWithNullEventsAggregateRootEntity() {
        Initialize(null);
      }
    }

    [TestFixture]
    public class WithInstanceToWhichNullIsApplied {
      [Test]
      public void ApplyWithNullAsEventThrows() {
        var sut = new WithPublicApplyAggregateRootEntity();
        Assert.Throws<ArgumentNullException>(sut.ApplyNull);
      }
    }

    class WithPublicApplyAggregateRootEntity : AggregateRootEntity {
      public void ApplyNull() {
        Apply(null);
      }
    }
  }
}
