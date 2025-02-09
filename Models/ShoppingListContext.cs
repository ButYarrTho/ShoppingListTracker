using Microsoft.EntityFrameworkCore;
using ShoppingListTracker.Models.DTO;

namespace ShoppingListTracker.Models
{
    public class ShoppingListContext : DbContext
    {
        public ShoppingListContext(DbContextOptions<ShoppingListContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define the relationship between Item and Category
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Category)  // Each item belongs to a category
                .WithMany(c => c.Items)   // Each cat can have many items
                .HasForeignKey(i => i.CategoryId); // this is the foreign key in the items table
        }
    }
}
