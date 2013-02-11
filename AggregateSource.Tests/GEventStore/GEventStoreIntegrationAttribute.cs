using System;
using NUnit.Framework;

namespace AggregateSource.GEventStore {
  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
  public class GEventStoreIntegrationAttribute : Attribute, ITestAction {
    public void BeforeTest(TestDetails testDetails) {
      EmbeddedEventStore.Instance.Start();
    }

    public void AfterTest(TestDetails testDetails) {
      EmbeddedEventStore.Instance.Stop();
    }

    public ActionTargets Targets { get { return ActionTargets.Suite; } }
  }
}