using System;
using NUnit.Framework;

namespace AggregateSource {
  namespace EntityTests {
    [TestFixture]
    public class WithAnyInstance {
      [Test]
      public void ApplierCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new UseNullApplierEntity());
      }

      [Test]
      public void PlayEventCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new PlayWithNullEventEntity());
      }

      [Test]
      public void ApplyEventCanNotBeNull() {
        var sut = new ApplyNullEventEntity();
        Assert.Throws<ArgumentNullException>(sut.ApplyNull);
      }

      [Test]
      public void RegisterHandlerCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new RegisterNullHandlerEntity());
      }

      [Test]
      public void RegisterHandlerCanOnlyBeCalledOncePerEventType() {
        Assert.Throws<ArgumentException>(() => new RegisterSameEventHandlerTwiceEntity());
      }
    }

    class UseNullApplierEntity : Entity {
      public UseNullApplierEntity() : base(null) {}
    }

    class PlayWithNullEventEntity : Entity {
      public PlayWithNullEventEntity() : base(_ => { }) {
        Play(null);
      }
    }

    class ApplyNullEventEntity : Entity {
      public ApplyNullEventEntity() : base(_ => { }) {}

      public void ApplyNull() {
        Apply(null);
      }
    }

    class RegisterNullHandlerEntity : Entity {
      public RegisterNullHandlerEntity() : base(_ => { }) {
        Register<object>(null);
      }
    }

    class RegisterSameEventHandlerTwiceEntity : Entity {
      public RegisterSameEventHandlerTwiceEntity() : base(_ => { }) {
        Register<object>(o => { });
        Register<object>(o => { });
      }
    }

    public class WithInstanceWithHandlers {}

    public class WithInstanceWithoutHandlers { }
  }
}
