using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using coa_Wallet.Models;

namespace coa_Wallet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.ParentCategory)
                .ToListAsync();
            return Ok(categories);
        }

        [HttpGet("hierarchy")]
        public async Task<IActionResult> GetCategoryHierarchy()
        {
            var categories = await _context.Categories
                .Include(c => c.ParentCategory)
                .ToListAsync();

            var rootCategories = categories
                .Where(c => c.ParentCategoryId == null)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    Subcategories = categories
                        .Where(sub => sub.ParentCategoryId == c.Id)
                        .Select(sub => new { sub.Id, sub.Name })
                });

            return Ok(rootCategories);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            if (id != category.Id) return BadRequest();
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
    }