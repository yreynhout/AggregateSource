using System;
using AggregateSource.GEventStore.Resolvers;
using EventStore.ClientAPI;
using NUnit.Framework;

namespace AggregateSource.GEventStore {
  [TestFixture]
  public class EventReaderConfigurationTests {
    [Test]
    public void DeserializerCannotBeNull() {
      Assert.Throws<ArgumentNullException>(() => CreateSutWithDeserializer(null));
    }

    [Test]
    public void ResolverCannotBeNull() {
      Assert.Throws<ArgumentNullException>(() => CreateSutWithResolver(null));
    }

    [Test]
    public void UsingConstructorReturnsInstanceWithExpectedProperties() {
      var resolvedEventDeserializer = new StubbedEventDeserializer();
      var sliceSize = new SliceSize(5);
      var streamNameResolver = new PassThroughStreamNameResolver();
      
      var result = CreateSut(sliceSize, resolvedEventDeserializer, streamNameResolver);

      Assert.That(result.Deserializer, Is.SameAs(resolvedEventDeserializer));
      Assert.That(result.SliceSize, Is.EqualTo(sliceSize));
      Assert.That(result.Resolver, Is.SameAs(streamNameResolver));
    }

    static EventReaderConfiguration CreateSut(SliceSize sliceSize, IEventDeserializer deserializer, IStreamNameResolver resolver) {
      return new EventReaderConfiguration(sliceSize, deserializer, resolver);
    }

    static EventReaderConfiguration CreateSutWithDeserializer(IEventDeserializer deserializer) {
      return CreateSut(new SliceSize(1), deserializer, new PassThroughStreamNameResolver());
    }

    static EventReaderConfiguration CreateSutWithResolver(IStreamNameResolver resolver) {
      return CreateSut(new SliceSize(1), new StubbedEventDeserializer(), resolver);
    }

    class StubbedEventDeserializer : IEventDeserializer {
      public object Deserialize(ResolvedEvent resolvedEvent) {
        return null;
      }
    }
  }
}
