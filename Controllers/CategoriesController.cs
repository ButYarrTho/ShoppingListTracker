using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingListTracker.Models;
using ShoppingListTracker.Models.DTO;


namespace ShoppingListTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ShoppingListContext _context;

        public CategoriesController(ShoppingListContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.Items)  // Make sure to include related items
                .ToListAsync();

            // Map the categories to CategoryDto and their items to ItemDto
            var categoryDtos = categories.Select(category => new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Items = category.Items.Select(item => new ItemDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            }).ToList();

            return categoryDtos;
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Items)  // Include related items
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            // Map the category to CategoryDto and its items to ItemDto
            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Items = category.Items.Select(item => new ItemDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            };

            return categoryDto;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, CategoryDto categoryDto)
        {
            if (id != categoryDto.Id)
            {
                return BadRequest();
            }

            var category = await _context.Categories
                .Include(c => c.Items)  // Include items to update them too
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            // Map CategoryDto to Category entity
            category.Name = categoryDto.Name;

            // Update items (remove old ones, add new ones)
            category.Items.Clear();
            category.Items.AddRange(categoryDto.Items.Select(i => new Item
            {
                Id = i.Id,
                Name = i.Name,
                Quantity = i.Quantity,
                Price = i.Price
            }));

            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(CategoryDto categoryDto)
        {
            // Map CategoryDto to Category entity
            var category = new Category
            {
                Name = categoryDto.Name,
                Items = categoryDto.Items.Select(i => new Item
                {
                    Name = i.Name,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
