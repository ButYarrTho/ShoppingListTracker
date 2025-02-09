namespace ShoppingListTracker.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; } // this is the foregn key
        public Category Category { get; set; } // for nav
    }
}
