namespace EventStoreShopping.Messaging.Commands
{
    public class IncrementItemCountInCart
    {
        public string CartId { get; set; }
        public string ItemId { get; set; }
    }
}