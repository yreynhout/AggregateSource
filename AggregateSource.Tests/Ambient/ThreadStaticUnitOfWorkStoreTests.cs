using AggregateSource.Ambient;
using AggregateSource.Tests.Ambient.AmbientUnitOfWorkStoreFixtures;
using NUnit.Framework;

namespace AggregateSource.Tests.Ambient {
  namespace ThreadStaticUnitOfWorkStoreTests {
    [TestFixture]
    public class WithAnyInstance : WithAnyInstanceFixture {
      protected override IAmbientUnitOfWorkStore CreateStore() {
        return new ThreadStaticUnitOfWorkStore();
      }
    }

    [TestFixture]
    public class WithPristineInstance : WithPristineInstanceFixture {
      protected override IAmbientUnitOfWorkStore CreateStore() {
        return new ThreadStaticUnitOfWorkStore();
      }
    }

    [TestFixture]
    public class WithSetInstance : WithSetInstanceFixture {
      protected override IAmbientUnitOfWorkStore CreateStore() {
        return new ThreadStaticUnitOfWorkStore();
      }
    }

    [TestFixture]
    public class WithClearedInstance : WithClearedInstanceFixture {
      protected override IAmbientUnitOfWorkStore CreateStore() {
        return new ThreadStaticUnitOfWorkStore();
      }
    }
  }
}
