using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using coa_Wallet.Models;

namespace coa_wallet.Controllers
{
 [ApiController]
    [Route("api/[controller]")]
    public class VisualizationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VisualizationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("spending-trend")]
        public async Task<IActionResult> GetSpendingTrend([FromQuery] int months = 6)
        {
            var startDate = DateTime.Now.AddMonths(-months);
            var trend = await _context.Transactions
                .Where(t => t.Date >= startDate)
                .GroupBy(t => new { t.Date.Year, t.Date.Month, t.Type })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Type = g.Key.Type,
                    Total = g.Sum(t => t.Amount)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            return Ok(trend);
        }

        [HttpGet("category-distribution")]
        public async Task<IActionResult> GetCategoryDistribution()
        {
            var distribution = await _context.Transactions
                .Where(t => t.Date.Month == DateTime.Now.Month)
                .GroupBy(t => t.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    Total = g.Sum(t => t.Amount),
                    Percentage = g.Sum(t => t.Amount) / 
                        _context.Transactions
                            .Where(t => t.Date.Month == DateTime.Now.Month)
                            .Sum(t => t.Amount) * 100
                })
                .ToListAsync();

            return Ok(distribution);
        }
    }
}   