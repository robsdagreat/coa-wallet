using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using coa_Wallet.Models;
using coa_Wallet.DTOs;

namespace coa_Wallet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            category.UserId = userId;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var categories = await _context.Categories
                .Include(c => c.ParentCategory)
                .Where(c => c.UserId == userId)
                .ToListAsync();
            return Ok(categories);
        }

        [HttpGet("hierarchy")]
        public async Task<IActionResult> GetCategoryHierarchy()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var categories = await _context.Categories
                .Include(c => c.ParentCategory)
                .Where(c => c.UserId == userId)
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
        public async Task<ActionResult<Category>> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (id != updateDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (category == null)
            {
                return NotFound($"Category with ID {id} not found");
            }

            // Update properties
            category.Name = updateDto.Name;
            category.ParentCategoryId = updateDto.ParentCategoryId;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(category);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CategoryExists(id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        private async Task<bool> CategoryExists(int id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }
    }
}