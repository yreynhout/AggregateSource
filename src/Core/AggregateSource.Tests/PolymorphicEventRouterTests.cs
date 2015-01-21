using System;
using NUnit.Framework;

namespace AggregateSource
{
    [TestFixture]
    public class PolymorphicEventRouterTests
    {
        PolymorphicEventRouter _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new PolymorphicEventRouter();
        }

        [Test]
        public void IsEventRouter()
        {
            Assert.That(_sut, Is.InstanceOf<IEventRouter>());
        }

        [Test]
        public void ConfigureRouteGenericHandlerCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.ConfigureRoute<object>(null));
        }

        [Test]
        public void CannotAddDuplicateRouteGenerically()
        {
            _sut.ConfigureRoute((object _) => { });
            Assert.Throws<ArgumentException>(() => _sut.ConfigureRoute((object _) => { }));
        }

        [Test]
        public void ConfigureRouteEventCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.ConfigureRoute(null, _ => { }));
        }

        [Test]
        public void ConfigureRouteHandlerCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.ConfigureRoute(typeof(object), null));
        }

        [Test]
        public void CannotAddDuplicateRoute()
        {
            _sut.ConfigureRoute(typeof(object), _ => { });
            Assert.Throws<ArgumentException>(() => _sut.ConfigureRoute(typeof(object), _ => { }));
        }

        [Test]
        public void RouteEventCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Route(null));
        }

        [Test]
        public void CanRouteEventWithoutHandler()
        {
            Assert.DoesNotThrow(() => _sut.Route(new object()));
        }

        [Test]
        public void RouteEventWithExactEventTypeHandlerHasExpectedResult()
        {
            var called = false;
            _sut.ConfigureRoute((object _) => called = true);

            _sut.Route(new object());

            Assert.That(called, Is.True);
        }

        [Test]
        public void RouteEventWithObjectTypeHandlerHasExpectedResult()
        {
            var called = false;
            _sut.ConfigureRoute((object _) => called = true);

            _sut.Route(new TestEvent());

            Assert.That(called, Is.True);
        }

        [Test]
        public void RouteEventWithEventBaseTypeHandlerHasExpectedResult()
        {
            var called = false;
            _sut.ConfigureRoute((BaseEvent _) => called = true);

            _sut.Route(new TestEventDerivedFromBaseEvent());

            Assert.That(called, Is.True);
        }

        [Test]
        public void RouteEventWithEventInterfaceTypeHandlerHasExpectedResult()
        {
            var called = false;
            _sut.ConfigureRoute((IEvent _) => called = true);

            _sut.Route(new TestEventDerivedFromIEvent());

            Assert.That(called, Is.True);
        }

        [Test]
        public void RouteEventWithMultipleHandlersHasExpectedResult()
        {
            var called1 = false;
            var called2 = false;
            var called3 = false;
            var called4 = false;
            var called5 = false;
            var called6 = false;
            _sut.ConfigureRoute((IEvent1 _) => called1 = true);
            _sut.ConfigureRoute((BaseEvent1 _) => called2 = true);
            _sut.ConfigureRoute((IEvent2 _) => called3 = true);
            _sut.ConfigureRoute((BaseEvent2 _) => called4 = true);
            _sut.ConfigureRoute((IEvent3 _) => called5 = true);
            _sut.ConfigureRoute((TestEventDerivedFromMultiple _) => called6 = true);

            _sut.Route(new TestEventDerivedFromMultiple());

            Assert.That(called1, Is.True);
            Assert.That(called2, Is.True);
            Assert.That(called3, Is.True);
            Assert.That(called4, Is.True);
            Assert.That(called5, Is.True);
            Assert.That(called6, Is.True);
        }

        class TestEvent { }

        abstract class BaseEvent { }
        class TestEventDerivedFromBaseEvent : BaseEvent { }

        interface IEvent { }
        class TestEventDerivedFromIEvent : IEvent { }

        interface IEvent1 { }
        abstract class BaseEvent1 : IEvent1 {}
        interface IEvent2 { }
        abstract class BaseEvent2 : BaseEvent1, IEvent2 { }
        interface IEvent3 { }
        class TestEventDerivedFromMultiple : BaseEvent2, IEvent3 { }
    }
}