namespace EventStoreShopping.Messaging.Events
{
    public class DecrementedItemCountInCart
    {
        public string CartId { get; set; }
        public string ItemId { get; set; }
    }
}