﻿namespace ShoppingListTracker.Models.DTO
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<ItemDto> Items { get; set; }
    }
}
