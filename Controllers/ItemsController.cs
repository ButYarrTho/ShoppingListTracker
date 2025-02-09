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
    public class ItemsController : ControllerBase
    {
        private readonly ShoppingListContext _context;

        public ItemsController(ShoppingListContext context)
        {
            _context = context;
        }

        // GET: api/Items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItems()
        {
            var items = await _context.Items
                .Include(item => item.Category) // Eager load Category to include category details
                .Select(item => new ItemDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Category = new CategoryDto // Map the Category object to CategoryDto
                    {
                        Id = item.Category.Id,
                        Name = item.Category.Name
                    }
                })
                .ToListAsync();

            return Ok(items);
        }

        // GET: api/Items/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItem(int id)
        {
            var item = await _context.Items
                .Include(i => i.Category) // Eager load Category
                .Where(i => i.Id == id)
                .Select(i => new ItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    Category = new CategoryDto // Map the Category object to CategoryDto
                    {
                        Id = i.Category.Id,
                        Name = i.Category.Name
                    }
                })
                .FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        // PUT: api/Items/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(int id, ItemDto itemDto)
        {
            if (id != itemDto.Id)
            {
                return BadRequest();
            }

            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            // Update the item based on the ItemDto
            item.Name = itemDto.Name;
            item.Quantity = itemDto.Quantity;
            item.Price = itemDto.Price;
            item.CategoryId = itemDto.Category.Id; // Update the CategoryId based on CategoryDto

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Items
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostItem(ItemDto itemDto)
        {
            var item = new Item
            {
                Name = itemDto.Name,
                Quantity = itemDto.Quantity,
                Price = itemDto.Price,
                CategoryId = itemDto.Category.Id // Use CategoryId from CategoryDto
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            // Return the created item as an ItemDto
            var createdItemDto = new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Quantity = item.Quantity,
                Price = item.Price,
                Category = new CategoryDto
                {
                    Id = item.CategoryId, // Return CategoryDto using the categoryId
                    Name = item.Category.Name
                }
            };

            return CreatedAtAction("GetItem", new { id = item.Id }, createdItemDto);
        }

        // DELETE: api/Items/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
    }
}
