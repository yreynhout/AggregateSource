using System;
using NUnit.Framework;

namespace AggregateSource
{
    [TestFixture]
    public class InstanceEventRouterTests
    {
        InstanceEventRouter _sut;

        [SetUp]
        public void SetUp()
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
        public void ConfigureRouteGenericHandlerCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.ConfigureRoute<object>(null));
        }

        [Test]
        public void CanNotAddDuplicateRouteGenerically()
        {
            _sut.ConfigureRoute((object _) => { });
            Assert.Throws<ArgumentException>(() => _sut.ConfigureRoute((object _) => { }));
        }

        [Test]
        public void ConfigureRouteEventCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.ConfigureRoute(null, _ => { }));
        }

        [Test]
        public void ConfigureRouteHandlerCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.ConfigureRoute(typeof (object), null));
        }

        [Test]
        public void CanNotAddDuplicateRoute()
        {
            _sut.ConfigureRoute(typeof (object), _ => { });
            Assert.Throws<ArgumentException>(() => _sut.ConfigureRoute(typeof (object), _ => { }));
        }

        [Test]
        public void RouteEventCanNotBeNull()
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