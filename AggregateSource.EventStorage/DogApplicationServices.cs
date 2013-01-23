using System;

namespace AggregateSource.EventStorage {
  public class DogApplicationServices : IHandle<RegisterBirthOfDogCommand>, IHandle<RegisterThatTheDogGotAShot> {
    readonly Repository<Dog> _dogRepository;

    public DogApplicationServices(Repository<Dog> dogRepository) {
      if (dogRepository == null) throw new ArgumentNullException("dogRepository");
      _dogRepository = dogRepository;
    }

    public void Handle(RegisterBirthOfDogCommand message) {
      _dogRepository.Add(
        message.DogId, 
        new Dog(message.DogId, message.NameOfDog, message.DateOfBirth));
    }

    public void Handle(RegisterThatTheDogGotAShot message) {
      var dog = _dogRepository.Get(message.DogId);
      dog.AdministerShotOf(message.MedicineUsed, message.DateOfShot);
    }
  }
}