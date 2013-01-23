using System;
using System.Net;
using EventStore.ClientAPI;
using NUnit.Framework;

namespace AggregateSource.EventStorage {
  [TestFixture]
  public class SampleUsage {
    [Test]
    public void Show() {
      using (var connection = EventStoreConnection.Create()) {
        connection.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1113));
        var handler = new EventStoreAwareHandler(connection);
        var dogId = Guid.NewGuid();

        var registerBirthOfDog = new RegisterBirthOfDogCommand(
          dogId,
          "Sparky",
          DateTime.Today.AddYears(-1));
        handler.Handle(registerBirthOfDog);

        var registerThatTheDogGotAShot = new RegisterThatTheDogGotAShot(dogId, "Cocaine", DateTime.Today);
        handler.Handle(registerThatTheDogGotAShot);
      }
    }
  }
}
