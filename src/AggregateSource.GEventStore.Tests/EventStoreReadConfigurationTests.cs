using System;
using AggregateSource.GEventStore.Resolvers;
using EventStore.ClientAPI;
using NUnit.Framework;

namespace AggregateSource.GEventStore {
  [TestFixture]
  public class EventStoreReadConfigurationTests {
    [Test]
    public void DeserializerCannotBeNull() {
      Assert.Throws<ArgumentNullException>(() => CreateSutWithDeserializer(null));
    }

    [Test]
    public void StreamNameResolverCannotBeNull() {
      Assert.Throws<ArgumentNullException>(() => CreateSutWithResolver(null));
    }

    [Test]
    public void UsingConstructorReturnsInstanceWithExpectedProperties() {
      var resolvedEventDeserializer = new StubbedResolvedEventDeserializer();
      var sliceSize = new SliceSize(5);
      var streamNameResolver = new PassThroughStreamNameResolver();
      
      var result = CreateSut(sliceSize, resolvedEventDeserializer, streamNameResolver);

      Assert.That(result.ResolvedEventDeserializer, Is.SameAs(resolvedEventDeserializer));
      Assert.That(result.SliceSize, Is.EqualTo(sliceSize));
      Assert.That(result.StreamNameResolver, Is.SameAs(streamNameResolver));
    }

    EventStoreReadConfiguration CreateSut(SliceSize sliceSize, IResolvedEventDeserializer deserializer, IStreamNameResolver resolver) {
      return new EventStoreReadConfiguration(sliceSize, deserializer, resolver);
    }

    EventStoreReadConfiguration CreateSutWithDeserializer(IResolvedEventDeserializer deserializer) {
      return CreateSut(new SliceSize(1), deserializer, new PassThroughStreamNameResolver());
    }

    EventStoreReadConfiguration CreateSutWithResolver(IStreamNameResolver resolver) {
      return CreateSut(new SliceSize(1), new StubbedResolvedEventDeserializer(), resolver);
    }

    class StubbedResolvedEventDeserializer : IResolvedEventDeserializer {
      public object Deserialize(ResolvedEvent resolvedEvent) {
        return null;
      }
    }
  }
}
