using System;
using ProtoBuf;

namespace AggregateSource.EventStorage {
  [ProtoContract]
  class DogGotAShot {
    [ProtoMember(1)]
    public Guid DogId { get; private set; }
    [ProtoMember(2)]
    public string MedicineUsed { get; private set; }
    [ProtoMember(3)]
    public DateTime DateOfShot { get; private set; }

    DogGotAShot() {}

    public DogGotAShot(Guid dogId, string medicineUsed, DateTime dateOfShot) {
      DogId = dogId;
      MedicineUsed = medicineUsed;
      DateOfShot = dateOfShot;
    }

    public override string ToString() {
      return string.Format("Hey, the dog got a shot of {0}.", MedicineUsed);
    }
  }
}