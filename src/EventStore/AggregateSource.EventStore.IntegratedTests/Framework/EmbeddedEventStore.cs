using System.IO;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using EventStore.Core.Tests.Helpers;

namespace AggregateSource.EventStore.Framework
{
    public static class EmbeddedEventStore
    {
        public static void Start()
        {
            var node = new MiniNode(Path.Combine(Path.GetTempPath(), "EventStore"), 1113, 1114, 2113, skipInitializeStandardUsersCheck: false);
            node.Start();
            Node = node;
            Credentials = new UserCredentials("admin", "changeit");
            var connection = EventStoreConnection.Create(
                ConnectionSettings.Create().EnableVerboseLogging().
                    SetDefaultUserCredentials(Credentials).
                    UseConsoleLogger(),
                node.TcpEndPoint);
            connection.Connect();
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
                node.Shutdown();
                Node = null;
            }
            Credentials = null;
        }

        public static MiniNode Node { get; private set; }

        public static IEventStoreConnection Connection { get; private set; }

        public static UserCredentials Credentials { get; private set; }
    }
}