using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource
{
    namespace EntityTests
    {
        [TestFixture]
        public class WithAnyInstance
        {
            [Test]
            public void IsInstanceEventRouter()
            {
                Assert.That(new AnyInstanceEntity(), Is.InstanceOf<IInstanceEventRouter>());
            }

            [Test]
            public void ApplierCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => new UseNullApplierEntity());
            }

            [Test]
            public void RouteEventCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => new RouteWithNullEventEntity());
            }

            [Test]
            public void ApplyEventCanNotBeNull()
            {
                var sut = new ApplyNullEventEntity();
                Assert.Throws<ArgumentNullException>(sut.ApplyNull);
            }
        }

        class AnyState
        {
            public void When(object @event) { }
        }

        class AnyInstanceEntity : Entity<AnyState> {
            public AnyInstanceEntity() : base(_ => { })
            {
            }
        }

        class UseNullApplierEntity : Entity<AnyState>
        {
            public UseNullApplierEntity() : base(null) {}
        }

        class RouteWithNullEventEntity : Entity<AnyState>
        {
            public RouteWithNullEventEntity() : base(_ => { })
            {
                Route(null);
            }
        }

        class ApplyNullEventEntity : Entity<AnyState>
        {
            public ApplyNullEventEntity() : base(_ => { }) {}

            public void ApplyNull()
            {
                Apply(null);
            }
        }

        [TestFixture]
        public class WithInstanceWithHandlers
        {
            WithHandlersEntity _sut;
            Action<object> _applier;
            List<object> _appliedEvents;

            [SetUp]
            public void SetUp()
            {
                _appliedEvents = new List<object>();
                _applier = _ => _appliedEvents.Add(_);
                _sut = new WithHandlersEntity(_applier);
            }

            [Test]
            public void RouteCallsHandlerOfEvent()
            {
                var expectedEvent = new object();

                _sut.Route(expectedEvent);

                Assert.That(_sut.RevealedState.HandlerCallCount, Is.EqualTo(1));
                Assert.That(_sut.RevealedState.RoutedEvents, Is.EquivalentTo(new[] { expectedEvent }));
            }

            [Test]
            public void ApplyEventCallsApplier()
            {
                var @event = new object();

                _sut.DoApply(@event);

                Assert.That(_appliedEvents, Is.EquivalentTo(new[] {@event}));
            }
        }

        class WithHandlersState
        {
            public WithHandlersState()
            {
                RoutedEvents = new List<object>();
            }

            public void When(object @event)
            {
                HandlerCallCount++;
                RoutedEvents.Add(@event);
            }

            public int HandlerCallCount { get; private set; }
            public List<object> RoutedEvents { get; private set; } 
        }

        class WithHandlersEntity : Entity<WithHandlersState>
        {
            public WithHandlersEntity(Action<object> applier)
                : base(applier)
            {
            }

            public void DoApply(object @event)
            {
                Apply(@event);
            }

            public WithHandlersState RevealedState { get { return State; } }
        }

        [TestFixture]
        public class WithInstanceWithoutHandlers
        {
            WithoutHandlersEntity _sut;
            Action<object> _applier;
            List<object> _appliedEvents;

            [SetUp]
            public void SetUp()
            {
                _appliedEvents = new List<object>();
                _applier = _ => _appliedEvents.Add(_);
                _sut = new WithoutHandlersEntity(_applier);
            }

            [Test]
            public void RouteDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.Route(new object()));
            }

            [Test]
            public void ApplyEventDoesNotThrow()
            {
                var @event = new object();

                _sut.DoApply(@event);

                Assert.That(_appliedEvents, Is.EquivalentTo(new[] {@event}));
            }
        }

        class WithoutHandlersState
        {
            public void When(object @event) { }
        }

        class WithoutHandlersEntity : Entity<WithoutHandlersState>
        {
            public WithoutHandlersEntity(Action<object> applier) : base(applier) {}

            public void DoApply(object @event)
            {
                Apply(@event);
            }
        }
    }
}