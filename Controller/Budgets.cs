using coa_Wallet.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
    [Route("api/[controller]")]
    public class BudgetsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BudgetsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SetBudget([FromBody] Budget budget)
        {
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();
            return Ok(budget);
        }

        [HttpGet("check")] // Check if budget exceeded
        public IActionResult CheckBudget([FromQuery] int accountId)
        {
            var budget = _context.Budgets.FirstOrDefault(b => b.AccountId == accountId);
            if (budget == null) return NotFound("Budget not set for this account.");

            var totalExpenses = _context.Transactions
                .Where(t => t.AccountId == accountId && t.Type == "Expense")
                .Sum(t => t.Amount);

            if (totalExpenses > budget.Limit)
                return Ok("Budget exceeded!");

            return Ok("Budget is within limits.");
        }
    }