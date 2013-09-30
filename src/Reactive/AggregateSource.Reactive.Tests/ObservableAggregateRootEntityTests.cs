using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AggregateSource.Reactive
{
    namespace ObservableAggregateRootEntityTests
    {
        [TestFixture]
        public class WithAnyInstance
        {
            [Test]
            public void IsObservableAggregateRootEntity()
            {
                Assert.IsInstanceOf<IObservableAggregateRootEntity>(new AnyAggregateRootEntity());
            }

            [Test]
            public void InitializeEventsCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => new InitializeWithNullEventsAggregateRootEntity());
            }

            [Test]
            public void ApplyEventCanNotBeNull()
            {
                var sut = new ApplyNullEventAggregateRootEntity();
                Assert.Throws<ArgumentNullException>(sut.ApplyNull);
            }

            [Test]
            public void ApplyCanBeInterceptedBeforeApplication()
            {
                var sut = new ApplyInterceptorAggregateRootEntity();
                Assert.That(sut.BeforeApplyWasCalled, Is.True);
            }

            [Test]
            public void ApplyCanBeInterceptedAfterApplication()
            {
                var sut = new ApplyInterceptorAggregateRootEntity();
                Assert.That(sut.AfterApplyWasCalled, Is.True);
            }

            [Test]
            public void RegisterHandlerCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => new RegisterNullHandlerAggregateRootEntity());
            }

            [Test]
            public void RegisterHandlerCanOnlyBeCalledOncePerEventType()
            {
                Assert.Throws<ArgumentException>(() => new RegisterSameEventHandlerTwiceAggregateRootEntity());
            }
        }

        class AnyAggregateRootEntity : ObservableAggregateRootEntity {}

        class InitializeWithNullEventsAggregateRootEntity : ObservableAggregateRootEntity
        {
            public InitializeWithNullEventsAggregateRootEntity()
            {
                Initialize(null);
            }
        }

        class ApplyNullEventAggregateRootEntity : ObservableAggregateRootEntity
        {
            public void ApplyNull()
            {
                Apply(null);
            }
        }

        class ApplyInterceptorAggregateRootEntity : ObservableAggregateRootEntity
        {
            public ApplyInterceptorAggregateRootEntity()
            {
                Register<object>(o => { });
                Apply(new object());
            }

            protected override void BeforeApply(object @event)
            {
                BeforeApplyWasCalled = true;
            }

            public bool BeforeApplyWasCalled { get; private set; }

            protected override void AfterApply(object @event)
            {
                AfterApplyWasCalled = true;
            }

            public bool AfterApplyWasCalled { get; private set; }
        }

        class RegisterNullHandlerAggregateRootEntity : ObservableAggregateRootEntity
        {
            public RegisterNullHandlerAggregateRootEntity()
            {
                Register<object>(null);
            }
        }

        class RegisterSameEventHandlerTwiceAggregateRootEntity : ObservableAggregateRootEntity
        {
            public RegisterSameEventHandlerTwiceAggregateRootEntity()
            {
                Register<object>(o => { });
                Register<object>(o => { });
            }
        }

        [TestFixture]
        public class WithPristineInstance
        {
            ObservableAggregateRootEntity _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new PristineAggregateRootEntity();
            }

            [Test]
            public void ClearChangesDoesNothing()
            {
                Assert.DoesNotThrow(() => _sut.ClearChanges());
            }

            [Test]
            public void HasChangesReturnsFalse()
            {
                Assert.That(_sut.HasChanges(), Is.False);
            }

            [Test]
            public void GetChangesReturnsEmpty()
            {
                Assert.That(_sut.GetChanges(), Is.EquivalentTo(Enumerable.Empty<object>()));
            }

            [Test]
            public void InitializeDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.Initialize(new[] {new object(), new object(), new object()}));
            }
        }

        class PristineAggregateRootEntity : ObservableAggregateRootEntity {}

        [TestFixture]
        public class WithInitializedInstance
        {
            ObservableAggregateRootEntity _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new InitializedAggregateRootEntity();
            }

            [Test]
            public void ClearChangesDoesNothing()
            {
                Assert.DoesNotThrow(() => _sut.ClearChanges());
            }

            [Test]
            public void HasChangesReturnsFalse()
            {
                Assert.That(_sut.HasChanges(), Is.False);
            }

            [Test]
            public void GetChangesReturnsEmpty()
            {
                Assert.That(_sut.GetChanges(), Is.EquivalentTo(Enumerable.Empty<object>()));
            }

            [Test]
            public void InitializeDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.Initialize(new[] {new object(), new object(), new object()}));
            }
        }

        class InitializedAggregateRootEntity : ObservableAggregateRootEntity
        {
            public InitializedAggregateRootEntity()
            {
                Initialize(new[] {new object(), new object()});
            }
        }

        [TestFixture]
        public class WithChangedInstance
        {
            ObservableAggregateRootEntity _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new ChangedAggregateRootEntity();
            }

            [Test]
            public void ClearChangesDoesNothing()
            {
                Assert.DoesNotThrow(() => _sut.ClearChanges());
            }

            [Test]
            public void HasChangesReturnsFalse()
            {
                Assert.That(_sut.HasChanges(), Is.True);
            }

            [Test]
            public void GetChangesReturnsEmpty()
            {
                Assert.That(_sut.GetChanges(), Is.EquivalentTo(ChangedAggregateRootEntity.AppliedChanges));
            }

            [Test]
            public void InitializeThrows()
            {
                Assert.Throws<InvalidOperationException>(
                    () => _sut.Initialize(new[] {new object(), new object(), new object()}));
            }
        }

        class ChangedAggregateRootEntity : ObservableAggregateRootEntity
        {
            public static readonly object[] AppliedChanges = new[] {new object(), new object()};

            public ChangedAggregateRootEntity()
            {
                foreach (var change in AppliedChanges)
                {
                    Apply(change);
                }
            }
        }

        [TestFixture]
        public class WithInitializedThenChangedInstance
        {
            ObservableAggregateRootEntity _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new InitializedThenChangedAggregateRootEntity();
            }

            [Test]
            public void ClearChangesDoesNothing()
            {
                Assert.DoesNotThrow(() => _sut.ClearChanges());
            }

            [Test]
            public void HasChangesReturnsFalse()
            {
                Assert.That(_sut.HasChanges(), Is.True);
            }

            [Test]
            public void GetChangesReturnsEmpty()
            {
                Assert.That(_sut.GetChanges(), Is.EquivalentTo(InitializedThenChangedAggregateRootEntity.AppliedChanges));
            }

            [Test]
            public void InitializeThrows()
            {
                Assert.Throws<InvalidOperationException>(
                    () => _sut.Initialize(new[] {new object(), new object(), new object()}));
            }
        }

        class InitializedThenChangedAggregateRootEntity : ObservableAggregateRootEntity
        {
            public static readonly object[] AppliedChanges = new[] {new object(), new object()};

            public InitializedThenChangedAggregateRootEntity()
            {
                Initialize(new[] {new object(), new object()});
                foreach (var change in AppliedChanges)
                {
                    Apply(change);
                }
            }
        }

        [TestFixture]
        public class WithChangedThenClearedInstance
        {
            ObservableAggregateRootEntity _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new ChangedThenClearedAggregateRootEntity();
            }

            [Test]
            public void ClearChangesDoesNothing()
            {
                Assert.DoesNotThrow(() => _sut.ClearChanges());
            }

            [Test]
            public void HasChangesReturnsFalse()
            {
                Assert.That(_sut.HasChanges(), Is.False);
            }

            [Test]
            public void GetChangesReturnsEmpty()
            {
                Assert.That(_sut.GetChanges(), Is.EquivalentTo(Enumerable.Empty<object>()));
            }

            [Test]
            public void InitializeDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.Initialize(new[] {new object(), new object(), new object()}));
            }
        }

        class ChangedThenClearedAggregateRootEntity : ObservableAggregateRootEntity
        {
            public ChangedThenClearedAggregateRootEntity()
            {
                foreach (var change in new[] {new object(), new object()})
                {
                    Apply(change);
                }
                ClearChanges();
            }
        }

        [TestFixture]
        public class WithInitializedThenChangedThenClearedInstance
        {
            ObservableAggregateRootEntity _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new InitializedThenChangedThenClearedAggregateRootEntity();
            }

            [Test]
            public void ClearChangesDoesNothing()
            {
                Assert.DoesNotThrow(() => _sut.ClearChanges());
            }

            [Test]
            public void HasChangesReturnsFalse()
            {
                Assert.That(_sut.HasChanges(), Is.False);
            }

            [Test]
            public void GetChangesReturnsEmpty()
            {
                Assert.That(_sut.GetChanges(), Is.EquivalentTo(Enumerable.Empty<object>()));
            }

            [Test]
            public void InitializeDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.Initialize(new[] {new object(), new object(), new object()}));
            }
        }

        class InitializedThenChangedThenClearedAggregateRootEntity : ObservableAggregateRootEntity
        {
            public InitializedThenChangedThenClearedAggregateRootEntity()
            {
                Initialize(new[] {new object(), new object()});
                foreach (var change in new[] {new object(), new object()})
                {
                    Apply(change);
                }
                ClearChanges();
            }
        }

        [TestFixture]
        public class WithInstanceWithHandlers
        {
            WithHandlersAggregateRootEntity _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new WithHandlersAggregateRootEntity();
            }

            [Test]
            public void InitializeCallsHandlerForEachEvent()
            {
                var expectedEvents = new[] {new object(), new object()};

                _sut.Initialize(expectedEvents);

                Assert.That(_sut.HandlerCallCount, Is.EqualTo(2));
                Assert.That(_sut.RoutedEvents, Is.EquivalentTo(expectedEvents));
            }

            [Test]
            public void ApplyEventCallsEventHandler()
            {
                var @event = new object();

                _sut.DoApply(@event);

                Assert.That(_sut.HandlerCallCount, Is.EqualTo(1));
                Assert.That(_sut.RoutedEvents, Is.EquivalentTo(new[] {@event}));
            }
        }

        class WithHandlersAggregateRootEntity : ObservableAggregateRootEntity
        {
            public WithHandlersAggregateRootEntity()
            {
                RoutedEvents = new List<object>();
                Register<object>(@event =>
                {
                    HandlerCallCount++;
                    RoutedEvents.Add(@event);
                });
            }

            public void DoApply(object @event)
            {
                Apply(@event);
            }

            public int HandlerCallCount { get; private set; }
            public List<object> RoutedEvents { get; private set; }
        }

        [TestFixture]
        public class WithInstanceWithoutHandlers
        {
            WithoutHandlersAggregateRootEntity _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new WithoutHandlersAggregateRootEntity();
            }

            [Test]
            public void InitializeDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.Initialize(new[] {new object(), new object()}));
            }

            [Test]
            public void ApplyEventDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.DoApply(new object()));
            }
        }

        class WithoutHandlersAggregateRootEntity : ObservableAggregateRootEntity
        {
            public void DoApply(object @event)
            {
                Apply(@event);
            }
        }
    }
}