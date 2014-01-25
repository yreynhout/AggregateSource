using System;
using NUnit.Framework;

namespace AggregateSource
{
    [TestFixture]
    public class FactTests
    {
        TestFactBuilder _sutBuilder;

        [SetUp]
        public void SetUp()
        {
            _sutBuilder = new TestFactBuilder();
        }

        [Test]
        public void IdentifierCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sutBuilder.WithIdentifier(null).Build());
        }

        [Test]
        public void EventCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sutBuilder.WithEvent(null).Build());
        }

        [Test]
        public void TwoInstancesAreEqualWhenTheirIdentifierAndEventAreEqual()
        {
            Assert.AreEqual(_sutBuilder.Build(), _sutBuilder.Build());
        }

        [Test]
        public void TwoInstancesAreNotEqualWhenTheirIdentifierDiffers()
        {
            Assert.AreNotEqual(
                _sutBuilder.WithIdentifier("123").Build(), 
                _sutBuilder.WithIdentifier("456").Build());
        }

        [Test]
        public void TwoInstancesAreNotEqualWhenTheirEventDiffers()
        {
            Assert.AreNotEqual(
                _sutBuilder.WithEvent(new object()).Build(),
                _sutBuilder.WithEvent(new object()).Build());
        }

        [Test]
        public void TwoInstancesHaveTheSameHashCodeWhenTheirIdentifierAndEventAreEqual()
        {
            Assert.AreEqual(_sutBuilder.Build().GetHashCode(), _sutBuilder.Build().GetHashCode());
        }

        [Test]
        public void TwoInstancesDoNotHaveTheSameHashCodeWhenTheirIdentifierDiffers()
        {
            Assert.AreNotEqual(
                _sutBuilder.WithIdentifier("123").Build().GetHashCode(),
                _sutBuilder.WithIdentifier("456").Build().GetHashCode());
        }

        [Test]
        public void TwoInstancesDoNotHaveTheSameHashCodeWhenTheirEventDiffers()
        {
            Assert.AreNotEqual(
                _sutBuilder.WithEvent(new object()).Build().GetHashCode(),
                _sutBuilder.WithEvent(new object()).Build().GetHashCode());
        }

        [Test]
        public void IsEquatable()
        {
            Assert.That(_sutBuilder.Build(), Is.InstanceOf<IEquatable<Fact>>());
        }

        [Test]
        public void DoesObjectEqualItself()
        {
            var instance = _sutBuilder.Build();
            Assert.IsTrue(instance.Equals((object)instance));
        }

        [Test]
        public void DoesNotEqualObjectOfOtherType()
        {
            Assert.IsFalse(_sutBuilder.Build().Equals(new object()));
        }

        [Test]
        public void DoesNotEqualNullAsObject()
        {
            Assert.IsFalse(_sutBuilder.Build().Equals(null));
        }

        [Test]
        public void UsingConstructorReturnsInstanceWithExpectedProperties()
        {
            var @event = new object();
            var sut = _sutBuilder.WithIdentifier("123").WithEvent(@event).Build();

            Assert.That(sut.Identifier, Is.EqualTo("123"));
            Assert.That(sut.Event, Is.SameAs(@event));
        }

        [Test]
        public void ImplicitConversionToTupleReturnsExpectedResult()
        {
            var @event = new object();
            var sut = _sutBuilder.WithIdentifier("123").WithEvent(@event).Build();

            Tuple<string, object> result = sut;

            Assert.That(result.Item1, Is.EqualTo("123"));
            Assert.That(result.Item2, Is.SameAs(@event));
        }
    }
}
