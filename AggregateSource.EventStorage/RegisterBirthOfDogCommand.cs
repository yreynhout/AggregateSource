using System;

namespace AggregateSource.EventStorage {
  public class RegisterBirthOfDogCommand {
    public readonly Guid DogId;
    public readonly string NameOfDog;
    public readonly DateTime DateOfBirth;

    public RegisterBirthOfDogCommand(Guid dogId, string nameOfDog, DateTime dateOfBirth) {
      DogId = dogId;
      NameOfDog = nameOfDog;
      DateOfBirth = dateOfBirth;
    }
  }
}