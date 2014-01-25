using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource
{
    namespace EventRecorderTests
    {
        [TestFixture]
        public class WithAnyInstance
        {
            EventRecorder _sut;

            [SetUp]
            public void Setup()
            {
                _sut = new EventRecorder();
            }

            [Test]
            public void IsEnumerable()
            {
                Assert.IsInstanceOf<IEnumerable<object>>(_sut);
            }

            [Test]
            public void RecordEventCannotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => _sut.Record(null));
            }
        }

        [TestFixture]
        public class WithEmptyInstance
        {
            EventRecorder _sut;

            [SetUp]
            public void Setup()
            {
                _sut = new EventRecorder();
            }

            [Test]
            public void ResetDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.Reset());
            }

            [Test]
            public void IsStillEmptyAfterReset()
            {
                _sut.Reset();
                Assert.IsEmpty(_sut);
            }

            [Test]
            public void IsEmpty()
            {
                Assert.IsEmpty(_sut);
            }

            [Test]
            public void RecordDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.Record(new object()));
            }

            [Test]
            public void ContainsExpectedEventsAfterRecord()
            {
                var initialEvent = new object();
                _sut.Record(initialEvent);
                Assert.That(_sut, Is.EquivalentTo(new[] {initialEvent}));
            }
        }

        [TestFixture]
        public class WithMutatedInstance
        {
            EventRecorder _sut;
            object _initialEvent;

            [SetUp]
            public void Setup()
            {
                _sut = new EventRecorder();
                _initialEvent = new object();
                _sut.Record(_initialEvent);
            }

            [Test]
            public void ResetDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.Reset());
            }

            [Test]
            public void IsEmptyAfterReset()
            {
                _sut.Reset();
                Assert.IsEmpty(_sut);
            }

            [Test]
            public void IsNotEmpty()
            {
                Assert.IsNotEmpty(_sut);
            }

            [Test]
            public void ContainsExpectedEvents()
            {
                Assert.That(_sut, Is.EquivalentTo(new[] {_initialEvent}));
            }

            [Test]
            public void RecordDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.Record(new object()));
            }

            [Test]
            public void ContainsExpectedEventsAfterRecord()
            {
                var nextEvent = new object();
                _sut.Record(nextEvent);
                Assert.That(_sut, Is.EquivalentTo(new[] {_initialEvent, nextEvent}));
            }
        }
    }
}