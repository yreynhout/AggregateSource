namespace EventStoreShopping.Messaging.Commands
{
    public class AddItemToCart
    {
        public string CartId { get; set; }
        public string ItemId { get; set; }
        public int Count { get; set; }
    }
}
