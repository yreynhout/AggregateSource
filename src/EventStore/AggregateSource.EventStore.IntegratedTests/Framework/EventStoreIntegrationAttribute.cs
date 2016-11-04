using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace AggregateSource.EventStore.Framework
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class EventStoreIntegrationAttribute : Attribute, ITestAction
    {
        public void BeforeTest(ITest test)
        {
            EmbeddedEventStore.Start();
        }

        public void AfterTest(ITest test)
        {
            EmbeddedEventStore.Stop();
        }

        public ActionTargets Targets
        {
            get { return ActionTargets.Suite; }
        }
    }
}