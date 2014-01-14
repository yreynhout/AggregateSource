namespace EventStoreShopping.Messaging.Events
{
    public class RemovedItemFromCart
    {
        public string CartId { get; set; }
        public string ItemId { get; set; }
    }
}