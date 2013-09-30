using System;
using NUnit.Framework;

namespace AggregateSource
{
    [TestFixture]
    public class StaticEventRouterTests
    {
        StaticEventRouter _sut;

        [SetUp]
        public void SetUp()
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
        public void ConfigureRouteGenericHandlerCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.ConfigureRoute<object, object>(null));
        }

        [Test]
        public void CanNotAddDuplicateRouteGenerically()
        {
            _sut.ConfigureRoute((object instance, object @event) => { });
            Assert.Throws<ArgumentException>(() => _sut.ConfigureRoute((object instance, object @event) => { }));
        }

        [Test]
        public void CanConfigureRouteGenericallyForSameEventTypeOnDifferentInstanceType()
        {
            _sut.ConfigureRoute((object instance, Event1 @event) => { });
            Assert.DoesNotThrow(() => _sut.ConfigureRoute((object instance, Event2 @event) => { }));
        }

        class Event1
        {}

        class Event2
        {}

        [Test]
        public void ConfigureRouteInstanceCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.ConfigureRoute(null, typeof (object), (instance, @event) => { }));
        }

        [Test]
        public void ConfigureRouteEventCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.ConfigureRoute(typeof (object), null, (instance, @event) => { }));
        }

        [Test]
        public void ConfigureRouteHandlerCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.ConfigureRoute(typeof (object), typeof (object), null));
        }

        [Test]
        public void CanNotAddDuplicateRoute()
        {
            _sut.ConfigureRoute(typeof (object), typeof (object), (instance, @event) => { });
            Assert.Throws<ArgumentException>(
                () => _sut.ConfigureRoute(typeof (object), typeof (object), (instance, @event) => { }));
        }

        [Test]
        public void RouteInstanceCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Route(null, new object()));
        }

        [Test]
        public void RouteEventCanNotBeNull()
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