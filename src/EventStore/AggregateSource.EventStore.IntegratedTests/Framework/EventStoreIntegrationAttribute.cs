using System;
using NUnit.Framework;

namespace AggregateSource.EventStore.Framework
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class EventStoreIntegrationAttribute : Attribute, ITestAction
    {
        public void BeforeTest(TestDetails testDetails)
        {
            EmbeddedEventStore.Start();
        }

        public void AfterTest(TestDetails testDetails)
        {
            EmbeddedEventStore.Stop();
        }

        public ActionTargets Targets
        {
            get { return ActionTargets.Suite; }
        }
    }
}