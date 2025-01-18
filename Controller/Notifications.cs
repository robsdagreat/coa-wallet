using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using coa_Wallet.Models;

namespace coa_Wallet.Controllers
{
  [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("budget-alerts")]
        public async Task<IActionResult> GetBudgetAlerts([FromQuery] int accountId)
        {
            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.AccountId == accountId);

            if (budget == null) return NotFound("No budget set for this account");

            var totalExpenses = await _context.Transactions
                .Where(t => t.AccountId == accountId && 
                           t.Type == "Expense" && 
                           t.Date.Month == DateTime.Now.Month)
                .SumAsync(t => t.Amount);

            var alert = new
            {
                BudgetLimit = budget.Limit,
                CurrentSpending = totalExpenses,
                RemainingBudget = budget.Limit - totalExpenses,
                IsExceeded = totalExpenses > budget.Limit,
                PercentageUsed = (totalExpenses / budget.Limit) * 100
            };

            return Ok(alert);
        }
    }
}   