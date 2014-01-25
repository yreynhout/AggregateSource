using System;
using NUnit.Framework;

namespace AggregateSource
{
    [TestFixture]
    public class StaticEventRouterTests
    {
        StaticEventRouter _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new StaticEventRouter();
        }

        [Test]
        public void IsStaticEventRouter()
        {
            Assert.That(_sut, Is.InstanceOf<IStaticEventRouter>());
        }

        [Test]
        public void IsConfigureStaticEventRouter()
        {
            Assert.That(_sut, Is.InstanceOf<IConfigureStaticEventRouter>());
        }

        [Test]
        public void ConfigureRouteGenericHandlerCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.ConfigureRoute<object, object>(null));
        }

        [Test]
        public void CannotAddDuplicateRouteGenerically()
        {
            _sut.ConfigureRoute((object instance, object @event) => { });
            Assert.Throws<ArgumentException>(() => _sut.ConfigureRoute((object instance, object @event) => { }));
        }

        [Test]
        public void CanConfigureRouteGenericallyForSameInstanceTypeButDifferentEventType()
        {
            _sut.ConfigureRoute((object instance, Event1 @event) => { });
            Assert.DoesNotThrow(() => _sut.ConfigureRoute((object instance, Event2 @event) => { }));
        }

        private abstract class Event1
        {}

        private abstract class Event2
        {}

        [Test]
        public void ConfigureRouteInstanceCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.ConfigureRoute(null, typeof (object), (instance, @event) => { }));
        }

        [Test]
        public void ConfigureRouteEventCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.ConfigureRoute(typeof (object), null, (instance, @event) => { }));
        }

        [Test]
        public void ConfigureRouteHandlerCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.ConfigureRoute(typeof (object), typeof (object), null));
        }

        [Test]
        public void CannotAddDuplicateRoute()
        {
            _sut.ConfigureRoute(typeof (object), typeof (object), (instance, @event) => { });
            Assert.Throws<ArgumentException>(
                () => _sut.ConfigureRoute(typeof (object), typeof (object), (instance, @event) => { }));
        }

        [Test]
        public void RouteInstanceCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Route(null, new object()));
        }

        [Test]
        public void RouteEventCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Route(new object(), null));
        }

        [Test]
        public void CanRouteEventWithoutHandler()
        {
            Assert.DoesNotThrow(() => _sut.Route(new object(), new object()));
        }

        [Test]
        public void RouteEventWithHandlerHasExpectedResult()
        {
            var called = false;
            _sut.ConfigureRoute((object instance, object @event) => called = true);

            _sut.Route(new object(), new object());

            Assert.That(called, Is.True);
        }
    }
}