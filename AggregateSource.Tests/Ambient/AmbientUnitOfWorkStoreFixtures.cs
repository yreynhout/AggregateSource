using System;
using AggregateSource.Ambient;
using NUnit.Framework;

namespace AggregateSource.Tests.Ambient {
  namespace AmbientUnitOfWorkStoreFixtures {
    public abstract class WithAnyInstanceFixture : InstanceBaseFixture {
      [Test]
      public void SetNullThrows() {
        Assert.Throws<ArgumentNullException>(() => Sut.Set(null));
      }
    }

    public abstract class WithPristineInstanceFixture : InstanceBaseFixture {
      [Test]
      public void SetDoesNotThrow() {
        Assert.DoesNotThrow(() => Sut.Set(new UnitOfWork()));
      }

      [Test]
      public void TryGetReturnsFalseAndNull() {
        UnitOfWork unitOfWork;

        var result = Sut.TryGet(out unitOfWork);

        Assert.That(result, Is.False);
        Assert.That(unitOfWork, Is.Null);
      }

      [Test]
      public void ClearThrows() {
        Assert.Throws<UnitOfWorkScopeException>(() => Sut.Clear());
      }
    }

    public abstract class WithSetInstanceFixture : InstanceBaseFixture {
      UnitOfWork _unitOfWork;

      public override void SetUp() {
        base.SetUp();
        _unitOfWork = new UnitOfWork();
        Sut.Set(_unitOfWork);
      }

      [Test]
      public void SetThrows() {
        Assert.Throws<UnitOfWorkScopeException>(() => Sut.Set(new UnitOfWork()));
      }

      [Test]
      public void TryGetReturnsTrueAndExpectedUnitOfWork() {
        UnitOfWork unitOfWork;

        var result = Sut.TryGet(out unitOfWork);

        Assert.That(result, Is.True);
        Assert.That(unitOfWork, Is.SameAs(_unitOfWork));
      }

      [Test]
      public void ClearDoesNotThrow() {
        Assert.DoesNotThrow(() => Sut.Clear());
      }
    }

    public abstract class WithClearedInstanceFixture : InstanceBaseFixture {
      UnitOfWork _unitOfWork;

      public override void SetUp() {
        base.SetUp();
        _unitOfWork = new UnitOfWork();
        Sut.Set(_unitOfWork);
        Sut.Clear();
      }

      [Test]
      public void SetDoesNotThrow() {
        Assert.DoesNotThrow(() => Sut.Set(new UnitOfWork()));
      }

      [Test]
      public void TryGetReturnsFalseAndNull() {
        UnitOfWork unitOfWork;

        var result = Sut.TryGet(out unitOfWork);

        Assert.That(result, Is.False);
        Assert.That(unitOfWork, Is.Null);
      }

      [Test]
      public void ClearThrows() {
        Assert.Throws<UnitOfWorkScopeException>(() => Sut.Clear());
      }
    }

    public abstract class InstanceBaseFixture {
      protected IAmbientUnitOfWorkStore Sut;

      protected abstract IAmbientUnitOfWorkStore CreateStore();

      [SetUp]
      public virtual void SetUp() {
        Sut = CreateStore();
      }

      [TearDown]
      public virtual void TearDown() {
        try {
          if (Sut != null) Sut.Clear();
        } catch { /* Just eat it, MJ */ }
      }
    }
  }
}
