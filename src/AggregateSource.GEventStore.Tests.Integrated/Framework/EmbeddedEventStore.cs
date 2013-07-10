using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using EventStore.ClientAPI;
using EventStore.Core;
using EventStore.Core.Bus;
using EventStore.Core.Messages;
using EventStore.Core.Services.Monitoring;
using EventStore.Core.Settings;
using EventStore.Core.TransactionLog.Checkpoint;
using EventStore.Core.TransactionLog.Chunks;
using EventStore.Core.TransactionLog.FileNamingStrategy;

namespace AggregateSource.GEventStore.Framework {
  public class EmbeddedEventStore {
    public static readonly EmbeddedEventStore Instance = new EmbeddedEventStore();

    static readonly IPEndPoint TcpEndPoint = new IPEndPoint(IPAddress.Loopback, 1113);

    SingleVNode _node;
    IEventStoreConnection _connection;

    public IEventStoreConnection Connection { get { return _connection; } }

    public void Start() {
      var db = CreateTFChunkDb();
      var settings = CreateSingleVNodeSettings();
      _node = new SingleVNode(db, settings, false, 1, new ISubsystem[0]);
      var waitHandle = new ManualResetEvent(false);
      _node.MainBus.Subscribe(new AdHocHandler<SystemMessage.BecomeMaster>(m => waitHandle.Set()));
      _node.Start();
      waitHandle.WaitOne();
      _connection = EventStoreConnection.Create(TcpEndPoint);
      _connection.Connect();
      //var logPath = Path.Combine(Path.GetDirectoryName(db.Config.Path), "logs");
      //if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);
      //EventStore.Common.Log.LogManager.Init("embedded-es", logPath);
    }

    public void Stop() {
      if (_connection != null) {
        _connection.Close();
      }
      if (_node != null) {
        //var waitHandle = new ManualResetEvent(false);
        //_node.Bus.Subscribe(new AdHocHandler<SystemMessage.BecomeShuttingDown>(m => waitHandle.WaitOne()));
        _node.Stop(false);
        //waitHandle.WaitOne();
      }
    }

    static TFChunkDb CreateTFChunkDb() {
      var dbPath = Path.Combine(Path.GetTempPath(), "EventStore", Guid.NewGuid().ToString("N"));
      if (!Directory.Exists(dbPath)) {
        Directory.CreateDirectory(dbPath);
      }
      var writerCheckFilename = Path.Combine(dbPath, Checkpoint.Writer + ".chk");
      var chaserCheckFilename = Path.Combine(dbPath, Checkpoint.Chaser + ".chk");
      var epochCheckFilename = Path.Combine(dbPath, Checkpoint.Epoch + ".chk");
      var truncateCheckFilename = Path.Combine(dbPath, Checkpoint.Truncate + ".chk");
      //Not mono friendly at this point.
      var writerChk = new MemoryMappedFileCheckpoint(writerCheckFilename, "writer", true);
      var chaserChk = new MemoryMappedFileCheckpoint(chaserCheckFilename, "chaser", true);
      var epochChk = new MemoryMappedFileCheckpoint(epochCheckFilename, Checkpoint.Epoch, cached: true, initValue: -1);
      var truncateChk = new MemoryMappedFileCheckpoint(truncateCheckFilename, Checkpoint.Truncate, cached: true, initValue: -1);
      const int cachedChunks = 100;
      var cache = cachedChunks * (long)(TFConsts.ChunkSize + ChunkHeader.Size + ChunkFooter.Size);
      return new TFChunkDb(
        new TFChunkDbConfig(
          dbPath,
          new VersionedPatternFileNamingStrategy(dbPath, "chunk-"),
          TFConsts.ChunkSize,
          cache,
          writerChk,
          chaserChk,
          epochChk,
          truncateChk
          ));
    }

    static SingleVNodeSettings CreateSingleVNodeSettings() {
      var settings = new SingleVNodeSettings(
        TcpEndPoint,
        new IPEndPoint(IPAddress.None, 0),
        new IPEndPoint(IPAddress.None, 0),
        new string[0], 
        false,
        new X509Certificate2(),
        1,
        0,
        TimeSpan.Zero,
        TimeSpan.Zero,
        TimeSpan.Zero,
        StatsStorage.None,
        false);
      return settings;
    }
  }
}