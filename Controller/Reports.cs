using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using coa_Wallet.Models;

namespace coa_Wallet.Controllers
{
[ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetTransactionsSummary([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .ToListAsync();

            var summary = new
            {
                TotalIncome = transactions.Where(t => t.Type == "Income").Sum(t => t.Amount),
                TotalExpenses = transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount),
                NetAmount = transactions.Where(t => t.Type == "Income").Sum(t => t.Amount) -
                           transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount)
            };

            return Ok(summary);
        }

        [HttpGet("by-category")]
        public async Task<IActionResult> GetCategoryReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var categoryTotals = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .GroupBy(t => new { t.Category.Name, t.Type })
                .Select(g => new
                {
                    Category = g.Key.Name,
                    Type = g.Key.Type,
                    Total = g.Sum(t => t.Amount)
                })
                .ToListAsync();

            return Ok(categoryTotals);
        }

        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyReport([FromQuery] int year)
        {
            var monthlyTotals = await _context.Transactions
                .Where(t => t.Date.Year == year)
                .GroupBy(t => new { t.Date.Month, t.Type })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Type = g.Key.Type,
                    Total = g.Sum(t => t.Amount)
                })
                .ToListAsync();

            return Ok(monthlyTotals);
        }
    }
}
