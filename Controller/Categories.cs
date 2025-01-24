// Remove duplicate using directives
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

            // Validate that AccountId is provided
            if (category.AccountId == 0)
            {
                return BadRequest("AccountId must be specified");
            }

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
                .ToListAsync();
            return Ok(categories);
        }

        [HttpGet("hierarchy")]
        public async Task<IActionResult> GetCategoryHierarchy()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var existingCategory = await _context.Categories
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCategory == null)
            {
                return NotFound();
            }

            if (existingCategory.Account.UserId != userId)
            {
                return Unauthorized();
            }

            if (id != category.Id) return BadRequest();
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

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

        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetCategoriesByAccount(int accountId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var categories = await _context.Categories
                .Where(c => c.AccountId == accountId)
                .Include(c => c.ParentCategory)
                .ToListAsync();

            // Transform to hierarchical structure
            var hierarchicalCategories = BuildCategoryHierarchy(categories);

            return Ok(hierarchicalCategories);
        }

        private List<HierarchicalCategory> BuildCategoryHierarchy(List<Category> categories)
        {
            var categoryDict = categories.ToDictionary(c => c.Id);
            var rootCategories = new List<HierarchicalCategory>();

            foreach (var category in categories)
            {
                var hierarchicalCategory = new HierarchicalCategory
                {
                    Id = category.Id,
                    Name = category.Name,
                    ParentCategoryId = category.ParentCategoryId
                };

                if (category.ParentCategoryId == null)
                {
                    rootCategories.Add(hierarchicalCategory);
                }
            }

            // Build children recursively
            foreach (var category in rootCategories)
            {
                category.Children = categories
                    .Where(c => c.ParentCategoryId == category.Id)
                    .Select(c => new HierarchicalCategory
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ParentCategoryId = c.ParentCategoryId,
                        Children = categories
                            .Where(child => child.ParentCategoryId == c.Id)
                            .Select(child => new HierarchicalCategory
                            {
                                Id = child.Id,
                                Name = child.Name,
                                ParentCategoryId = child.ParentCategoryId
                            })
                            .ToList()
                    })
                    .ToList();
            }

            return rootCategories;
        }
            }
    }