using System;
using NUnit.Framework;

namespace AggregateSource
{
    [TestFixture]
    public class InstanceEventRouterTests
    {
        InstanceEventRouter _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new InstanceEventRouter();
        }

        [Test]
        public void IsInstanceEventRouter()
        {
            Assert.That(_sut, Is.InstanceOf<IInstanceEventRouter>());
        }

        [Test]
        public void IsConfigureInstanceEventRouter()
        {
            Assert.That(_sut, Is.InstanceOf<IConfigureInstanceEventRouter>());
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
            Assert.Throws<ArgumentNullException>(() => _sut.ConfigureRoute(typeof (object), null));
        }

        [Test]
        public void CannotAddDuplicateRoute()
        {
            _sut.ConfigureRoute(typeof (object), _ => { });
            Assert.Throws<ArgumentException>(() => _sut.ConfigureRoute(typeof (object), _ => { }));
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
        public void RouteEventWithHandlerHasExpectedResult()
        {
            var called = false;
            _sut.ConfigureRoute((object _) => called = true);

            _sut.Route(new object());

            Assert.That(called, Is.True);
        }
    }
}