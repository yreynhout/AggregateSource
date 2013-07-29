using System;
using NUnit.Framework;

namespace AggregateSource
{
    [TestFixture]
    public class InstanceEventRouterTests
    {
        private InstanceEventRouter _sut;

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
        public void AddRouteGenericHandlerCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.AddRoute<object>(null));
        }

        [Test]
        public void CanNotAddDuplicateRouteGenerically()
        {
            _sut.AddRoute((object _) => {});
            Assert.Throws<ArgumentException>(() => _sut.AddRoute((object _) => { }));
        }

        [Test]
        public void AddRouteEventCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.AddRoute(null, _ => { }));
        }

        [Test]
        public void AddRouteHandlerCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.AddRoute(typeof(object), null));
        }

        [Test]
        public void CanNotAddDuplicateRoute()
        {
            _sut.AddRoute(typeof(object), _ => { });
            Assert.Throws<ArgumentException>(() => _sut.AddRoute(typeof(object), _ => { }));
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
            _sut.AddRoute((object _)=> called = true);
            
            _sut.Route(new object());

            Assert.That(called, Is.True);
        }
    }
}