using System;
using AggregateSource.Ambient;
using NUnit.Framework;

namespace AggregateSource.Tests.Ambient {
  namespace UnitOfWorkScopeTests {
    [TestFixture]
    public class Construction {
      [Test]
      public void UnitOfWorkCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new UnitOfWorkScope(null, new ThreadStaticUnitOfWorkStore()));
      }

      [Test]
      public void StoreCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new UnitOfWorkScope(new UnitOfWork(), null));
      }
    }

    [TestFixture]
    public class WithoutScopedInstance {
      [Test]
      public void UseOfConstructorDoesNotThrow() {
        Assert.DoesNotThrow(() => {
          using (new UnitOfWorkScope(new UnitOfWork(), new ThreadStaticUnitOfWorkStore())) { }
        });
      }
    }

    [TestFixture]
    public class WithScopedInstance {
      UnitOfWork _unitOfWork;
      UnitOfWorkScope _sut;

      [SetUp]
      public void SetUp() {
        _unitOfWork = new UnitOfWork();
        _sut = new UnitOfWorkScope(_unitOfWork, new ThreadStaticUnitOfWorkStore());
      }

      [TearDown]
      public void TearDown() {
        if (_sut != null) _sut.Dispose();
      }

      [Test]
      public void UseOfConstructorThrows() {
        Assert.Throws<UnitOfWorkScopeException>(() => new UnitOfWorkScope(new UnitOfWork(), new ThreadStaticUnitOfWorkStore()));
      }

      [Test]
      public void DisposeDoesNotThrow() {
        Assert.DoesNotThrow(() => _sut.Dispose());
      }
    }

    [TestFixture]
    public class WithDisposedInstance {
      UnitOfWork _unitOfWork;
      UnitOfWorkScope _sut;

      [SetUp]
      public void SetUp() {
        _unitOfWork = new UnitOfWork();
        _sut = new UnitOfWorkScope(_unitOfWork, new ThreadStaticUnitOfWorkStore());
        _sut.Dispose();
      }

      [Test]
      public void UseOfConstructorDoesNotThrow() {
        Assert.DoesNotThrow(() => {
          using (new UnitOfWorkScope(new UnitOfWork(), new ThreadStaticUnitOfWorkStore())) { }
        });
      }
    }
  }
}
