namespace EventStoreShopping.Messaging.Events
{
    public class IncrementedItemCountInCart
    {
        public string CartId { get; set; }
        public string ItemId { get; set; }
    }
}