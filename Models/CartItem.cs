namespace ShoppingApp.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int ItemId { get; set; }
        public Item? Item { get; set; }

        public int Quantity { get; set; }
    }
}
