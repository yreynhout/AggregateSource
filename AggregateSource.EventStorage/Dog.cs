using System;

namespace AggregateSource.EventStorage {
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