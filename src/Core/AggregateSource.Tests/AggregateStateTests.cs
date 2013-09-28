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
                Assert.That(new AnyInstanceAggregateState(), Is.InstanceOf<IInstanceEventRouter>());
            }

            [Test]
            public void RouteEventCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => new RouteWithNullEventAggregateState());
            }

            [Test]
            public void RegisterHandlerCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => new RegisterNullHandlerAggregateState());
            }

            [Test]
            public void RegisterHandlerCanOnlyBeCalledOncePerEventType()
            {
                Assert.Throws<ArgumentException>(() => new RegisterSameEventHandlerTwiceAggregateState());
            }
        }

        class AnyInstanceAggregateState : AggregateState {}

        class RouteWithNullEventAggregateState : AggregateState
        {
            public RouteWithNullEventAggregateState()
            {
                Route(null);
            }
        }

        class RegisterNullHandlerAggregateState : AggregateState
        {
            public RegisterNullHandlerAggregateState()
            {
                Register<object>(null);
            }
        }

        class RegisterSameEventHandlerTwiceAggregateState : AggregateState
        {
            public RegisterSameEventHandlerTwiceAggregateState()
            {
                Register<object>(o => { });
                Register<object>(o => { });
            }
        }

        [TestFixture]
        public class WithInstanceWithHandlers
        {
            private WithHandlersAggregateState _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new WithHandlersAggregateState();
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

        class WithHandlersAggregateState : AggregateState
        {
            public WithHandlersAggregateState()
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
            private WithoutHandlersAggregateState _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new WithoutHandlersAggregateState();
            }

            [Test]
            public void RouteDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.Route(new object()));
            }
        }

        class WithoutHandlersAggregateState : AggregateState
        {
        }
    }
}