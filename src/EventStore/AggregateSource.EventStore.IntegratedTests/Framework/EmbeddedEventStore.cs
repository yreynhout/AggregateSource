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
            var tcs = new TaskCompletionSource<object>();
            node.NodeStatusChanged += (sender, args) =>
            {
                if (args.NewVNodeState == VNodeState.Master)
                    tcs.SetResult(null);
            };
            tcs.Task.Wait();
            Node = node;
            Credentials = new UserCredentials("admin", "changeit");
            var connection = EmbeddedEventStoreConnection.Create(Node);

            // This does not work, because ... ††† JEZUS †††
            //var connection = EventStoreConnection.Create(
            //    ConnectionSettings.Create().SetDefaultUserCredentials(Credentials).UseDebugLogger(),
            //    new IPEndPoint(Opts.InternalIpDefault, Opts.ExternalTcpPortDefault));
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