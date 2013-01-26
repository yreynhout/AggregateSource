using System.IO;
using System.Web;
using AggregateSource.Ambient;
using AggregateSource.Tests.Ambient.AmbientUnitOfWorkStoreFixtures;
using NUnit.Framework;

namespace AggregateSource.Tests.Ambient {
  namespace HttpContextUnitOfWorkStoreTests {
    [TestFixture]
    public class WithAnyInstance : WithAnyInstanceFixture {
      public override void SetUp() {
        HttpContext.Current = HttpContextStubFactory.Create();
        base.SetUp();
      }

      public override void TearDown() {
        base.TearDown();
        HttpContext.Current = null;
      }

      protected override IAmbientUnitOfWorkStore CreateStore() {
        return new HttpContextUnitOfWorkStore();
      }
    }

    [TestFixture]
    public class WithPristineInstance : WithPristineInstanceFixture {
      public override void SetUp() {
        HttpContext.Current = HttpContextStubFactory.Create();
        base.SetUp();
      }

      public override void TearDown() {
        base.TearDown();
        HttpContext.Current = null;
      }

      protected override IAmbientUnitOfWorkStore CreateStore() {
        return new HttpContextUnitOfWorkStore();
      }
    }

    [TestFixture]
    public class WithSetInstance : WithSetInstanceFixture {
      public override void SetUp() {
        HttpContext.Current = HttpContextStubFactory.Create();
        base.SetUp();
      }

      public override void TearDown() {
        base.TearDown();
        HttpContext.Current = null;
      }

      protected override IAmbientUnitOfWorkStore CreateStore() {
        return new HttpContextUnitOfWorkStore();
      }
    }

    [TestFixture]
    public class WithClearedInstance : WithClearedInstanceFixture {
      public override void SetUp() {
        HttpContext.Current = HttpContextStubFactory.Create();
        base.SetUp();
      }

      public override void TearDown() {
        base.TearDown();
        HttpContext.Current = null;
      }

      protected override IAmbientUnitOfWorkStore CreateStore() {
        return new HttpContextUnitOfWorkStore();
      }
    }

    static class HttpContextStubFactory {
      public static HttpContext Create() {
        return new HttpContext(new HttpRequest("", "http://_", ""), new HttpResponse(new StringWriter()));
      }
    }
  }
}
