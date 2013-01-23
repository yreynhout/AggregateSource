using System;
using ProtoBuf;

namespace AggregateSource.EventStorage {
  [ProtoContract]
  class DogWasBorn {
    [ProtoMember(1)]
    public Guid DogId { get; private set; }
    [ProtoMember(2)]
    public string NameOfDog { get; private set; }
    [ProtoMember(3)]
    public DateTime DateOfBirth { get; private set; }

    DogWasBorn() {}

    public DogWasBorn(Guid dogId, string nameOfDog, DateTime dateOfBirth) {
      DogId = dogId;
      NameOfDog = nameOfDog;
      DateOfBirth = dateOfBirth;
    }

    public override string ToString() {
      return string.Format("Yo, a dog called {0} was born on {1}", NameOfDog, DateOfBirth);
    }
  }
}