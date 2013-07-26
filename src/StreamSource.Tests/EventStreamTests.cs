using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace StreamSource
{
    [TestFixture]
    public class EventStreamTests
    {
        [Test]
        public void EventsCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EventStream(0, null));
        }

        [Datapoint] public IEnumerable<object> NoEvents = new object[0];

        [Datapoint] public IEnumerable<object> OneEvent = new[] {new object()};

        [Datapoint] public IEnumerable<object> TwoEvents = new[] {new object(), new object()};

        [Theory]
        public void UsingDefaultConstructorReturnsInstanceWithExpectedProperties(
            [Values(Int32.MinValue, -1, 0, 1, Int32.MaxValue)] int expectedVersion,
            IEnumerable<object> events)
        {
            var sut = new EventStream(expectedVersion, events);

            Assert.That(sut.ExpectedVersion, Is.EqualTo(expectedVersion));
            Assert.That(sut.Events, Is.SameAs(events));
        }
    }
}