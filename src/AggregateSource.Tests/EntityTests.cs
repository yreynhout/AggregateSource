using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AggregateSource
{
    namespace EntityTests
    {
        [TestFixture]
        public class WithAnyInstance
        {
            [Test]
            public void ApplierCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => new UseNullApplierEntity());
            }

            [Test]
            public void PlayEventCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => new PlayWithNullEventEntity());
            }

            [Test]
            public void ApplyEventCanNotBeNull()
            {
                var sut = new ApplyNullEventEntity();
                Assert.Throws<ArgumentNullException>(sut.ApplyNull);
            }

            [Test]
            public void RegisterHandlerCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => new RegisterNullHandlerEntity());
            }

            [Test]
            public void RegisterHandlerCanOnlyBeCalledOncePerEventType()
            {
                Assert.Throws<ArgumentException>(() => new RegisterSameEventHandlerTwiceEntity());
            }
        }

        internal class UseNullApplierEntity : Entity
        {
            public UseNullApplierEntity() : base(null)
            {
            }
        }

        internal class PlayWithNullEventEntity : Entity
        {
            public PlayWithNullEventEntity() : base(_ => { })
            {
                Play(null);
            }
        }

        internal class ApplyNullEventEntity : Entity
        {
            public ApplyNullEventEntity() : base(_ => { })
            {
            }

            public void ApplyNull()
            {
                Apply(null);
            }
        }

        internal class RegisterNullHandlerEntity : Entity
        {
            public RegisterNullHandlerEntity() : base(_ => { })
            {
                Register<object>(null);
            }
        }

        internal class RegisterSameEventHandlerTwiceEntity : Entity
        {
            public RegisterSameEventHandlerTwiceEntity() : base(_ => { })
            {
                Register<object>(o => { });
                Register<object>(o => { });
            }
        }

        [TestFixture]
        public class WithInstanceWithHandlers
        {
            private WithHandlersEntity _sut;
            private Action<object> _applier;
            private List<object> _appliedEvents;

            [SetUp]
            public void SetUp()
            {
                _appliedEvents = new List<object>();
                _applier = _ => _appliedEvents.Add(_);
                _sut = new WithHandlersEntity(_applier);
            }

            [Test]
            public void PlayCallsHandlerOfEvent()
            {
                var expectedEvent = new object();

                _sut.Play(expectedEvent);

                Assert.That(_sut.HandlerCallCount, Is.EqualTo(1));
                Assert.That(_sut.PlayedEvents, Is.EquivalentTo(new[] {expectedEvent}));
            }

            [Test]
            public void ApplyEventCallsApplier()
            {
                var @event = new object();

                _sut.DoApply(@event);

                Assert.That(_appliedEvents, Is.EquivalentTo(new[] {@event}));
            }
        }

        internal class WithHandlersEntity : Entity
        {
            public WithHandlersEntity(Action<object> applier)
                : base(applier)
            {
                PlayedEvents = new List<object>();
                Register<object>(@event =>
                {
                    HandlerCallCount++;
                    PlayedEvents.Add(@event);
                });
            }

            public void DoApply(object @event)
            {
                Apply(@event);
            }

            public int HandlerCallCount { get; private set; }
            public List<object> PlayedEvents { get; private set; }
        }

        [TestFixture]
        public class WithInstanceWithoutHandlers
        {
            private WithoutHandlersEntity _sut;
            private Action<object> _applier;
            private List<object> _appliedEvents;

            [SetUp]
            public void SetUp()
            {
                _appliedEvents = new List<object>();
                _applier = _ => _appliedEvents.Add(_);
                _sut = new WithoutHandlersEntity(_applier);
            }

            [Test]
            public void PlayDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.Play(new object()));
            }

            [Test]
            public void ApplyEventDoesNotThrow()
            {
                var @event = new object();

                _sut.DoApply(@event);

                Assert.That(_appliedEvents, Is.EquivalentTo(new[] {@event}));
            }
        }

        internal class WithoutHandlersEntity : Entity
        {
            public WithoutHandlersEntity(Action<object> applier) : base(applier)
            {
            }

            public void DoApply(object @event)
            {
                Apply(@event);
            }
        }
    }
}