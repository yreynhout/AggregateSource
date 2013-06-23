using System;
using System.Linq;
using System.Reflection;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Messages;
using NUnit.Framework;

namespace AggregateSource.GEventStore.Resolvers {
  [TestFixture]
  public class MetadataSnapshotVersionResolverTests {
    MetadataSnapshotVersionResolver _sut;
    
    [SetUp]
    public void SetUp() { _sut = new MetadataSnapshotVersionResolver(); }

    [Test]
    public void IsSnapshotVersionResolver() {
      Assert.That(_sut, Is.InstanceOf<ISnapshotVersionResolver>());
    }

    [Test]
    public void ResolveReturnsExpectedResult([Random(Int32.MinValue, Int32.MinValue, 1)]int value) {
      var result = _sut.Resolve(CreateResolvedEventWithMetadata(value));
      Assert.That(result, Is.EqualTo(value));
    }

    static ResolvedEvent CreateResolvedEventWithMetadata(int value) {
      var resolvedEventAsMessage = new ClientMessage.ResolvedEvent(
        new ClientMessage.EventRecord(
          "eventstreamid", 
          1, 
          Guid.NewGuid().ToByteArray(), 
          "eventtype", 
          new byte[0],
          BitConverter.GetBytes(value)),
        null, 0L, 0L);
      return (ResolvedEvent)
        typeof (ResolvedEvent).
        GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).
        Single(_ => _.GetParameters().Any(p => p.ParameterType == typeof (ClientMessage.ResolvedEvent))).
        Invoke(new object[] {resolvedEventAsMessage});
    }
  }
}
