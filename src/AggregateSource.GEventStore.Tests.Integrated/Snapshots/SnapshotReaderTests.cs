using System;
using AggregateSource.GEventStore.Framework;
using FakeItEasy;
using NUnit.Framework;

namespace AggregateSource.GEventStore.Snapshots {
  namespace SnapshotReaderTests {
    [TestFixture]
    public class WithAnyInstance {
      SnapshotStoreReadConfiguration _configuration;

      [SetUp]
      public void SetUp() {
        _configuration = new SnapshotStoreReadConfiguration(
          A.Fake<IStreamNameResolver>(),
          A.Fake<ISnapshotDeserializer>());
      }

      [Test]
      public void ConnectionCannotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new SnapshotReader(null, _configuration));
      }

      [Test]
      public void ConfigurationCannotBeNull() {
        Assert.Throws<ArgumentNullException>(() => new SnapshotReader(EmbeddedEventStore.Instance.Connection, null));
      }

      [Test]
      public void IsSnapshotReader() {
        Assert.That(new SnapshotReader(EmbeddedEventStore.Instance.Connection, _configuration), Is.InstanceOf<ISnapshotReader>());
      }
    }

    [TestFixture]
    public class WithSnapshotStreamFoundInStore {
      
    }

    [TestFixture]
    public class WithSnapshotStreamNotFoundInStore {
      
    }

    [TestFixture]
    public class WithEmptySnapshotStreamInStore { }
  }
}
