using System;
using System.Linq;
using AggregateSource;
using NUnit.Framework;

namespace StreamSource {
  // ReSharper disable MemberCanBePrivate.Local
  // ReSharper disable LocalizableElement
  [TestFixture]
  public class SampleUsage {
    [Test]
    public void Show() {
      //Somewhere in an application service wrapper
      var unitOfWork = new UnitOfWork();
      //Dependency of a domain service or application service
      var dogRepository = new Repository<Dog>(Dog.Factory, unitOfWork, id => null);
      //Application service handler code
      var dog = new Dog(Guid.NewGuid(), "Sparky", DateTime.Today.AddYears(-1));
      dog.AdministerShotOf("Anti Diarrhea Medicine", DateTime.Today);
      dogRepository.Add(dog.DogId, dog);
      //Back in the application service wrapper
      Console.WriteLine("We observed that:");
      foreach (var change in unitOfWork.GetChanges().SelectMany(aggregate => aggregate.Root.GetChanges())) {
        Console.WriteLine(change);
      }
    }

    class DogWasBorn {
      public readonly Guid DogId;
      public readonly string NameOfDog;

      public readonly DateTime DateOfBirth;
      public DogWasBorn(Guid dogId, string nameOfDog, DateTime dateOfBirth) {
        DogId = dogId;
        NameOfDog = nameOfDog;
        DateOfBirth = dateOfBirth;
      }

      public override string ToString() {
        return string.Format("Yo, a dog called {0} was born on {1}", NameOfDog, DateOfBirth);
      }
    }

    class DogGotAShot {
      public readonly Guid DogId;
      public readonly string MedicineUsed;
      public readonly DateTime DateOfShot;
      public DogGotAShot(Guid dogId, string medicineUsed, DateTime dateOfShot) {
        DogId = dogId;
        MedicineUsed = medicineUsed;
        DateOfShot = dateOfShot;
      }

      public override string ToString() {
        return string.Format("Hey, the dog got a shot of {0}.", MedicineUsed);
      }
    }

    public class Dog : AggregateRootEntity {
      Guid _dogId;

      public static readonly Func<Dog> Factory = () => new Dog();

      Dog() {
        Register<DogWasBorn>(ApplyEvent);
      }

      public Guid DogId {
        get { return _dogId; }
      }

      void ApplyEvent(DogWasBorn @event) {
        _dogId = @event.DogId;
      }

      public Dog(Guid dogId, string nameOfDog, DateTime dateOfBirth)
        : this() {
        Apply(new DogWasBorn(dogId, nameOfDog, dateOfBirth));
      }

      public void AdministerShotOf(string medicineUsed, DateTime dateOfShot) {
        Apply(new DogGotAShot(_dogId, medicineUsed, dateOfShot));
      }
    }
  }
  // ReSharper restore LocalizableElement
  // ReSharper restore MemberCanBePrivate.Local
}