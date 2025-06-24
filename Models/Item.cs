namespace ShoppingApp.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        // New property for image URL
        public string ImageUrl { get; set; } = "/images/default.png"; // default image path
    }

}
