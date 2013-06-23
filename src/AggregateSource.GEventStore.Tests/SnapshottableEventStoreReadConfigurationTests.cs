//using System;
//using EventStore.ClientAPI;
//using NUnit.Framework;

//namespace AggregateSource.GEventStore {
//  [TestFixture]
//  public class SnapshottableEventStoreReadConfigurationTests {
//    [Test]
//    public void DeserializerCannotBeNull() {
//      Assert.Throws<ArgumentNullException>(() => CreateSutWithDeserializer(null));
//    }

//    [Test]
//    public void UsingConstructorReturnsInstanceWithExpectedProperties() {
//      var deserializer = new StubbedResolvedEventDeserializer();
//      var sliceSize = new SliceSize(5);

//      var result = CreateSut(sliceSize, deserializer);

//      Assert.That(result.ResolvedEventDeserializer, Is.SameAs(deserializer));
//      Assert.That(result.SliceSize, Is.EqualTo(sliceSize));
//    }

//    EventStoreReadConfiguration CreateSut(SliceSize sliceSize, IResolvedEventDeserializer deserializer) {
//      return new EventStoreReadConfiguration(sliceSize, deserializer, TODO);
//    }

//    EventStoreReadConfiguration CreateSutWithDeserializer(IResolvedEventDeserializer deserializer) {
//      return CreateSut(new SliceSize(1), deserializer);
//    }

//    class StubbedResolvedEventDeserializer : IResolvedEventDeserializer {
//      public object Deserialize(ResolvedEvent resolvedEvent) {
//        return null;
//      }
//    }
//  }
//}