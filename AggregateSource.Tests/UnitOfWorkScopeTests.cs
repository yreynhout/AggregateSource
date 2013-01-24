using System;
using NUnit.Framework;

namespace AggregateSource.Tests {
  namespace UnitOfWorkScopeTests {
    [TestFixture]
    public class Construction {
      [Test]
      public void UnitOfWorkCanNotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new UnitOfWorkScope(null));
      }
    }

    [TestFixture]
    public class WithoutScopedInstance {
      [Test]
      public void IsScopedIsFalse() {
        Assert.IsFalse(UnitOfWorkScope.IsScoped);
      }

      [Test]
      public void CurrentThrows() {
        Assert.Throws<UnitOfWorkScopeException>(() => { var current = UnitOfWorkScope.Current; });
      }

      [Test]
      public void TryGetCurrentReturnsFalseAndNull() {
        UnitOfWorkScope scope;
        var result = UnitOfWorkScope.TryGetCurrent(out scope);

        Assert.That(result, Is.False);
        Assert.That(scope, Is.Null);
      }

      [Test]
      public void UseOfConstructorDoesNotThrow() {
        Assert.DoesNotThrow(() => {
          using(new UnitOfWorkScope(new UnitOfWork())){}
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
        _sut = new UnitOfWorkScope(_unitOfWork);
      }

      [TearDown]
      public void TearDown() {
        if(_sut != null) _sut.Dispose();
      }

      [Test]
      public void UnitOfWorkReturnsScopedUnitOfWork() {
        Assert.That(_sut.UnitOfWork, Is.SameAs(_unitOfWork));
      }

      [Test]
      public void IsScopedIsTrue() {
        Assert.IsTrue(UnitOfWorkScope.IsScoped);
      }

      [Test]
      public void CurrentReturnsScope() {
        Assert.That(UnitOfWorkScope.Current, Is.SameAs(_sut));
      }

      [Test]
      public void TryGetCurrentReturnsTrueAndScope() {
        UnitOfWorkScope scope;
        var result = UnitOfWorkScope.TryGetCurrent(out scope);

        Assert.That(result, Is.True);
        Assert.That(scope, Is.SameAs(_sut));
      }

      [Test]
      public void UseOfConstructorThrows() {
        Assert.Throws<UnitOfWorkScopeException>(() => new UnitOfWorkScope(new UnitOfWork()));
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
        _sut = new UnitOfWorkScope(_unitOfWork);
        _sut.Dispose();
      }

      [Test]
      public void IsScopedIsFalse() {
        Assert.IsFalse(UnitOfWorkScope.IsScoped);
      }

      [Test]
      public void CurrentThrows() {
        Assert.Throws<UnitOfWorkScopeException>(() => { var current = UnitOfWorkScope.Current; });
      }

      [Test]
      public void TryGetCurrentReturnsFalseAndNull() {
        UnitOfWorkScope scope;
        var result = UnitOfWorkScope.TryGetCurrent(out scope);

        Assert.That(result, Is.False);
        Assert.That(scope, Is.Null);
      }

      [Test]
      public void UseOfConstructorDoesNotThrow() {
        Assert.DoesNotThrow(() => {
          using (new UnitOfWorkScope(new UnitOfWork())) { }
        });
      }
    }
  }
}
