using System;

namespace AggregateSource.EventStorage {
  public class RegisterThatTheDogGotAShot {
    public readonly Guid DogId;
    public readonly string MedicineUsed;
    public readonly DateTime DateOfShot;

    public RegisterThatTheDogGotAShot(Guid dogId, string medicineUsed, DateTime dateOfShot) {
      DogId = dogId;
      MedicineUsed = medicineUsed;
      DateOfShot = dateOfShot;
    }
  }
}