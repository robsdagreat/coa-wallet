using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using coa_Wallet.Models;
using coa_Wallet.DTOs;
using coa_Wallet.DTOs;

namespace coa_Wallet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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

            // Removed category.UserId = userId as it's no longer part of the Category model
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
            if (id != category.Id) return BadRequest();
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
    }