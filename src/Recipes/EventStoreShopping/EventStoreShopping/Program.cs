using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using AggregateSource;
using AggregateSource.EventStore;
using AggregateSource.EventStore.Resolvers;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using EventStoreShopping.Shopping;
using Newtonsoft.Json;

namespace EventStoreShopping
{
    class Program
    {
        static void Main()
        {
            //Make sure you start an instance of EventStore before running this!!

            var credentials = new UserCredentials("admin", "changeit");
            var connection = EventStoreConnection.Create(
                ConnectionSettings.Create().
                    UseConsoleLogger().
                    SetDefaultUserCredentials(
                        credentials),
                new IPEndPoint(IPAddress.Loopback, 1113),
                "EventStoreShopping");
            connection.Connect();

            var unitOfWork = new UnitOfWork();
            var repository = new Repository<ShoppingCart>(
                ShoppingCart.Factory,
                unitOfWork,
                connection,
                new EventReaderConfiguration(
                    new SliceSize(512),
                    new JsonDeserializer(),
                    new PassThroughStreamNameResolver(),
                    new FixedStreamUserCredentialsResolver(credentials)));

            //Handle Start Shopping command
            var id = new ShoppingCartId("shopping_at_boutique_" + DateTime.UtcNow.Ticks);
            repository.Add(id, new ShoppingCart(id));

            //Handle Add Item command
            var cart1 = repository.Get(id);
            cart1.AddItem(new ItemId("pair_of_socks"), 2);

            //Handle Add Item command
            var cart2 = repository.Get(id);
            cart2.AddItem(new ItemId("pair_of_shirts"), 4);

            //Handle Remove Item command
            var cart3 = repository.Get(id);
            cart3.RemoveItem(new ItemId("pair_of_socks"));

            //Handle Increment Item Count command
            var cart4 = repository.Get(id);
            cart4.IncrementItemCount(new ItemId("pair_of_shirts"));

            //Handle Checkout command
            var cart5 = repository.Get(id);
            cart5.Checkout();

            //Append to stream
            var affected = unitOfWork.GetChanges().Single();
            connection.AppendToStream(
                affected.Identifier,
                affected.ExpectedVersion,
                affected.Root.GetChanges().
                    Select(_ =>
                        new EventData(
                            Guid.NewGuid(),
                            _.GetType().Name,
                            true,
                            ToJsonByteArray(_),
                            new byte[0])),
                credentials);
        }

        class JsonDeserializer : IEventDeserializer
        {
            public IEnumerable<object> Deserialize(ResolvedEvent resolvedEvent)
            {
                var type = Type.GetType(resolvedEvent.Event.EventType, true);
                using (var stream = new MemoryStream(resolvedEvent.Event.Data))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        yield return JsonSerializer.CreateDefault().Deserialize(reader, type);
                    }
                }
            }
        }

        static byte[] ToJsonByteArray(object @event)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    JsonSerializer.CreateDefault().Serialize(writer, @event);
                    writer.Flush();
                }
                return stream.ToArray();
            }
        }
    }
}
