using System;

namespace AggregateSource.Testing {
  public static class Scenario {
    public static IGivenStateBuilder Given(Guid id, params object[] events) {
      return new TestSpecificationBuilder().Given(id, events);
    }

    public static IWhenStateBuilder When(object message) {
      return new TestSpecificationBuilder().When(message);
    }

    internal static void Usage() {
      Guid A1Id = Guid.NewGuid();
      Guid A2Id = Guid.NewGuid();

      Scenario.
        Given(A1Id,
              new object(),
              new object()).
        //extensibility: GivenADogWasBorn().
        Given(A2Id,
              new object(),
              new object()).
        //extensibility: GivenTheDogGotTwoShots().
        When(new object()).
        //extensibility: WhenAdministeringANewShot().
        Then(A2Id,
             new object()).
        //extensibility: ThenAShotWasAdministered().
        Build();
        //extensibility: Assert(); / AssertThrows(Exception); => unit testing fx integration
        
    }
  }
}