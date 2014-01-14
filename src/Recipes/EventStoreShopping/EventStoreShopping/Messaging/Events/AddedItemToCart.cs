namespace EventStoreShopping.Messaging.Events
{
    public class AddedItemToCart
    {
        public string CartId { get; set; }
        public string ItemId { get; set; }
        public int Count { get; set; }
    }
}
