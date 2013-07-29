using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AggregateSource
{
    namespace AggregateRootEntityTests
    {
        [TestFixture]
        public class WithAnyInstance
        {
            [Test]
            public void IsObservableAggregateRootEntity()
            {
                Assert.IsInstanceOf<IAggregateRootEntity>(new AnyAggregateRootEntity());
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

        internal class AnyAggregateRootEntity : AggregateRootEntity
        {
        }

        internal class InitializeWithNullEventsAggregateRootEntity : AggregateRootEntity
        {
            public InitializeWithNullEventsAggregateRootEntity()
            {
                Initialize(null);
            }
        }

        internal class ApplyNullEventAggregateRootEntity : AggregateRootEntity
        {
            public void ApplyNull()
            {
                Apply(null);
            }
        }

        internal class ApplyInterceptorAggregateRootEntity : AggregateRootEntity
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

        internal class RegisterNullHandlerAggregateRootEntity : AggregateRootEntity
        {
            public RegisterNullHandlerAggregateRootEntity()
            {
                Register<object>(null);
            }
        }

        internal class RegisterSameEventHandlerTwiceAggregateRootEntity : AggregateRootEntity
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
            private AggregateRootEntity _sut;

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

        internal class PristineAggregateRootEntity : AggregateRootEntity
        {
        }

        [TestFixture]
        public class WithInitializedInstance
        {
            private AggregateRootEntity _sut;

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

        internal class InitializedAggregateRootEntity : AggregateRootEntity
        {
            public InitializedAggregateRootEntity()
            {
                Initialize(new[] {new object(), new object()});
            }
        }

        [TestFixture]
        public class WithChangedInstance
        {
            private AggregateRootEntity _sut;

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

        internal class ChangedAggregateRootEntity : AggregateRootEntity
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
            private AggregateRootEntity _sut;

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

        internal class InitializedThenChangedAggregateRootEntity : AggregateRootEntity
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
            private AggregateRootEntity _sut;

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

        internal class ChangedThenClearedAggregateRootEntity : AggregateRootEntity
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
            private AggregateRootEntity _sut;

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

        internal class InitializedThenChangedThenClearedAggregateRootEntity : AggregateRootEntity
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
            private WithHandlersAggregateRootEntity _sut;

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
                Assert.That(_sut.PlayedEvents, Is.EquivalentTo(expectedEvents));
            }

            [Test]
            public void ApplyEventCallsEventHandler()
            {
                var @event = new object();

                _sut.DoApply(@event);

                Assert.That(_sut.HandlerCallCount, Is.EqualTo(1));
                Assert.That(_sut.PlayedEvents, Is.EquivalentTo(new[] {@event}));
            }
        }

        internal class WithHandlersAggregateRootEntity : AggregateRootEntity
        {
            public WithHandlersAggregateRootEntity()
            {
                PlayedEvents = new List<object>();
                Register<object>(@event =>
                {
                    HandlerCallCount++;
                    PlayedEvents.Add(@event);
                });
            }

            public void DoApply(object @event)
            {
                Apply(@event);
            }

            public int HandlerCallCount { get; private set; }
            public List<object> PlayedEvents { get; private set; }
        }

        [TestFixture]
        public class WithInstanceWithoutHandlers
        {
            private WithoutHandlersAggregateRootEntity _sut;

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

        internal class WithoutHandlersAggregateRootEntity : AggregateRootEntity
        {
            public void DoApply(object @event)
            {
                Apply(@event);
            }
        }
    }
}