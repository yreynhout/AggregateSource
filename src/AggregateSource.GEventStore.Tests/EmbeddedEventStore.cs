using System;
using System.IO;
using System.Net;
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

namespace AggregateSource.GEventStore {
  public class EmbeddedEventStore {
    public static readonly EmbeddedEventStore Instance = new EmbeddedEventStore();

    static readonly IPEndPoint TcpEndPoint = new IPEndPoint(IPAddress.Loopback, 1113);

    SingleVNode _node;
    EventStoreConnection _connection;

    public EventStoreConnection Connection { get { return _connection; } }

    public void Start() {
      var db = CreateTFChunkDb();
      var settings = CreateSingleVNodeSettings();
      _node = new SingleVNode(db, settings, false);
      var waitHandle = new ManualResetEvent(false);
      _node.Bus.Subscribe(new AdHocHandler<SystemMessage.BecomeWorking>(m => waitHandle.Set()));
      _node.Start();
      waitHandle.WaitOne();
      _connection = EventStoreConnection.Create();
      _connection.Connect(TcpEndPoint);
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
        _node.Stop();
        //waitHandle.WaitOne();
      }
    }

    static TFChunkDb CreateTFChunkDb() {
      var dbPath = Path.Combine(Path.GetTempPath(), "EventStore", Guid.NewGuid().ToString("N"));
      if (!Directory.Exists(dbPath)) {
        Directory.CreateDirectory(dbPath);
      }
      var writerCheckFilename = Path.Combine(dbPath, "writer.chk");
      var chaserCheckFilename = Path.Combine(dbPath, "chaser.chk");
      //Not mono friendly at this point.
      var writerChk = new MemoryMappedFileCheckpoint(writerCheckFilename, "writer", true);
      var chaserChk = new MemoryMappedFileCheckpoint(chaserCheckFilename, "chaser", true);
      return new TFChunkDb(
        new TFChunkDbConfig(
          dbPath,
          new VersionedPatternFileNamingStrategy(dbPath, "chunk-"),
          0x10000000,
          2,
          writerChk,
          chaserChk,
          new ICheckpoint[] { writerChk, chaserChk }
          ));
    }

    static SingleVNodeSettings CreateSingleVNodeSettings() {
      var settings = new SingleVNodeSettings(
        TcpEndPoint,
        new IPEndPoint(IPAddress.None, 0),
        new string[0],
        1,
        1,
        3,
        TimeSpan.MaxValue,
        StatsStorage.None);
      return settings;
    }
  }
}