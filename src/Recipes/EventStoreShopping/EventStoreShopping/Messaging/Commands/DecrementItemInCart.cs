namespace EventStoreShopping.Messaging.Commands
{
    public class DecrementItemCountInCart
    {
        public string CartId { get; set; }
        public string ItemId { get; set; }
        public int NumberOfItems { get; set; }
    }
}