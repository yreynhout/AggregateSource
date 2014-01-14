namespace EventStoreShopping.Messaging.Commands
{
    public class RemoveItemFromCart
    {
        public string CartId { get; set; }
        public string ItemId { get; set; }
    }
}