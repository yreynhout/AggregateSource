using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using EventStore.Common.Log;
using EventStore.Core;
using EventStore.Core.Bus;
using EventStore.Core.Messages;
using EventStore.Core.Services.Monitoring;
using EventStore.Core.Settings;
using EventStore.Core.TransactionLog.Checkpoint;
using EventStore.Core.TransactionLog.Chunks;
using EventStore.Core.TransactionLog.FileNamingStrategy;
using EventStore.Core.Util;

namespace AggregateSource.GEventStore.Framework
{
    public class EmbeddedEventStore
    {
        public static readonly EmbeddedEventStore Instance = new EmbeddedEventStore();

        static readonly IPEndPoint TcpEndPoint = new IPEndPoint(IPAddress.Loopback, 1113);
        static readonly IPEndPoint HttpEndPoint = new IPEndPoint(IPAddress.Loopback, 2113);

        SingleVNode _node;
        IEventStoreConnection _connection;
        UserCredentials _credentials;

        public IEventStoreConnection Connection
        {
            get { return _connection; }
        }

        public UserCredentials DefaultCredentials
        {
            get { return _credentials; }
        }

        public void Start()
        {
            if (EmbeddedEventStoreConfiguration.RunWithLogging)
            {
                if (!Directory.Exists(EmbeddedEventStoreConfiguration.LogPath))
                    Directory.CreateDirectory(EmbeddedEventStoreConfiguration.LogPath);
                LogManager.Init(string.Format("as-embed-es-{0}", DateTime.Now.Ticks), EmbeddedEventStoreConfiguration.LogPath);
            }

            var db = CreateTFChunkDb(EmbeddedEventStoreConfiguration.StoragePath);
            var settings = CreateSingleVNodeSettings();
            _node = new SingleVNode(db, settings, false, 0xf4240, new ISubsystem[0]);
            var waitHandle = new ManualResetEvent(false);
            _node.MainBus.Subscribe(new AdHocHandler<SystemMessage.BecomeMaster>(m => waitHandle.Set()));
            _node.Start();
            waitHandle.WaitOne();
            _credentials = new UserCredentials("admin", "changeit");
            _connection = EventStoreConnection.Create(
                ConnectionSettings.Create().
                                   EnableVerboseLogging().
                                   SetDefaultUserCredentials(_credentials).
                                   UseConsoleLogger(),
                TcpEndPoint);
            _connection.Connect();
        }

        public void Stop()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
            if (_node != null)
            {
                _node.Stop(false);
            }
        }

        static TFChunkDb CreateTFChunkDb(string storagePath)
        {
            var dbPath = Path.Combine(storagePath, DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture));
            if (!Directory.Exists(dbPath))
            {
                Directory.CreateDirectory(dbPath);
            }
            var writerCheckFilename = Path.Combine(dbPath, Checkpoint.Writer + ".chk");
            var chaserCheckFilename = Path.Combine(dbPath, Checkpoint.Chaser + ".chk");
            var epochCheckFilename = Path.Combine(dbPath, Checkpoint.Epoch + ".chk");
            var truncateCheckFilename = Path.Combine(dbPath, Checkpoint.Truncate + ".chk");
            //Not mono friendly at this point.
            var writerChk = new MemoryMappedFileCheckpoint(writerCheckFilename, "writer", true);
            var chaserChk = new MemoryMappedFileCheckpoint(chaserCheckFilename, "chaser", true);
            var epochChk = new MemoryMappedFileCheckpoint(epochCheckFilename, Checkpoint.Epoch, cached: true,
                                                          initValue: -1);
            var truncateChk = new MemoryMappedFileCheckpoint(truncateCheckFilename, Checkpoint.Truncate, cached: true,
                                                             initValue: -1);
            const int cachedChunks = 100;
            return new TFChunkDb(new TFChunkDbConfig(dbPath,
                                                     new VersionedPatternFileNamingStrategy(dbPath, "chunk-"),
                                                     0x10000000, cachedChunks*0x10000100L, writerChk, chaserChk,
                                                     epochChk, truncateChk));
        }

        static SingleVNodeSettings CreateSingleVNodeSettings()
        {
            var settings = new SingleVNodeSettings(
                TcpEndPoint,
                null,
                new IPEndPoint(IPAddress.None, 0),
                new[] {string.Format("http://{0}:{1}/", HttpEndPoint.Address, HttpEndPoint.Port)},
                false,
                null,
                Opts.WorkerThreadsDefault,
                Opts.MinFlushDelayMsDefault,
                TimeSpan.FromMilliseconds(Opts.PrepareTimeoutMsDefault),
                TimeSpan.FromMilliseconds(Opts.CommitTimeoutMsDefault),
                TimeSpan.FromMilliseconds(Opts.StatsPeriodDefault),
                StatsStorage.None,
                false);
            return settings;
        }
    }
}