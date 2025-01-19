using coa_Wallet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Verify the account belongs to the user
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == budget.AccountId && a.UserId == userId);
            
            if (account == null)
            {
                return NotFound("Account not found or unauthorized");
            }

            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();
            return Ok(budget);
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckBudget([FromQuery] int accountId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Verify account ownership
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == accountId && a.UserId == userId);
            
            if (account == null)
            {
                return NotFound("Account not found or unauthorized");
            }

            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.AccountId == accountId);
                
            if (budget == null) 
                return NotFound("Budget not set for this account.");

            var totalExpenses = await _context.Transactions
                .Where(t => t.AccountId == accountId && 
                            t.Type == "Expense")
                .SumAsync(t => t.Amount);

            var remainingBudget = budget.Limit - totalExpenses;
            
            return Ok(new {
                budgetLimit = budget.Limit,
                totalExpenses = totalExpenses,
                remainingBudget = remainingBudget,
                isExceeded = totalExpenses > budget.Limit,
                percentageUsed = (totalExpenses / budget.Limit) * 100
            });
        }
    }