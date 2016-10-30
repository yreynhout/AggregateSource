using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Embedded;
using EventStore.ClientAPI.SystemData;
using EventStore.Core;
using EventStore.Core.Data;

namespace AggregateSource.EventStore.Framework
{
    public static class EmbeddedEventStore
    {
        public static void Start()
        {
            var node = EmbeddedVNodeBuilder.
                AsSingleNode().
                OnDefaultEndpoints().
                RunInMemory().
                Build();
            node.Start();
            node.StartAndWaitUntilReady().Wait();
            Node = node;
            Credentials = new UserCredentials("admin", "changeit");
            var settings = ConnectionSettings.Create()
                .SetDefaultUserCredentials(Credentials)
                .KeepReconnecting()
                .KeepRetrying()
                .UseConsoleLogger()
                .Build();
            var connection = EmbeddedEventStoreConnection.Create(Node, settings);
            connection.ConnectAsync().Wait();
            Connection = connection;
        }

        public static void Stop()
        {
            var connection = Connection;
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
                Connection = null;
            }
            var node = Node;
            if (node != null)
            {
                node.Stop();
                Node = null;
            }
            Credentials = null;
        }

        public static ClusterVNode Node { get; private set; }

        public static IEventStoreConnection Connection { get; private set; }

        public static UserCredentials Credentials { get; private set; }
    }
}