using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource
{
    namespace AggregateStateTests
    {
        [TestFixture]
        public class WithAnyInstance
        {
            [Test]
            public void IsInstanceEventRouter()
            {
                Assert.That(new AnyInstanceEntityState(), Is.InstanceOf<IInstanceEventRouter>());
            }

            [Test]
            public void RouteEventCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => new RouteWithNullEventEntityState());
            }

            [Test]
            public void RegisterHandlerCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => new RegisterNullHandlerEntityState());
            }

            [Test]
            public void RegisterHandlerCanOnlyBeCalledOncePerEventType()
            {
                Assert.Throws<ArgumentException>(() => new RegisterSameEventHandlerTwiceEntityState());
            }
        }

        class AnyInstanceEntityState : EntityState {}

        class RouteWithNullEventEntityState : EntityState
        {
            public RouteWithNullEventEntityState()
            {
                Route(null);
            }
        }

        class RegisterNullHandlerEntityState : EntityState
        {
            public RegisterNullHandlerEntityState()
            {
                Register<object>(null);
            }
        }

        class RegisterSameEventHandlerTwiceEntityState : EntityState
        {
            public RegisterSameEventHandlerTwiceEntityState()
            {
                Register<object>(o => { });
                Register<object>(o => { });
            }
        }

        [TestFixture]
        public class WithInstanceWithHandlers
        {
            private WithHandlersEntityState _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new WithHandlersEntityState();
            }

            [Test]
            public void RouteCallsHandlerOfEvent()
            {
                var expectedEvent = new object();

                _sut.Route(expectedEvent);

                Assert.That(_sut.HandlerCallCount, Is.EqualTo(1));
                Assert.That(_sut.RoutedEvents, Is.EquivalentTo(new[] {expectedEvent}));
            }
        }

        class WithHandlersEntityState : EntityState
        {
            public WithHandlersEntityState()
            {
                RoutedEvents = new List<object>();
                Register<object>(@event =>
                {
                    HandlerCallCount++;
                    RoutedEvents.Add(@event);
                });
            }

            public int HandlerCallCount { get; private set; }
            public List<object> RoutedEvents { get; private set; }
        }

        [TestFixture]
        public class WithInstanceWithoutHandlers
        {
            private WithoutHandlersEntityState _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new WithoutHandlersEntityState();
            }

            [Test]
            public void RouteDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.Route(new object()));
            }
        }

        class WithoutHandlersEntityState : EntityState
        {
        }
    }
}