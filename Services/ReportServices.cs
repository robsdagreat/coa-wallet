using coa_Wallet.Models;
using Microsoft.EntityFrameworkCore;
namespace coa_Wallet.Services
{
    public class ReportingService
    {
        private readonly AppDbContext _context;

        public ReportingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> GenerateDetailedReport(DateTime startDate, DateTime endDate, int accountId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.AccountId == accountId && 
                           t.Date >= startDate && 
                           t.Date <= endDate)
                .Include(t => t.Category)
                .ToListAsync();

            return new
            {
                Summary = new
                {
                    TotalIncome = transactions.Where(t => t.Type == "Income").Sum(t => t.Amount),
                    TotalExpenses = transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount),
                    NetAmount = transactions.Where(t => t.Type == "Income").Sum(t => t.Amount) -
                               transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount)
                },
                CategoryBreakdown = transactions
                    .GroupBy(t => new { t.Category.Name, t.Type })
                    .Select(g => new
                    {
                        Category = g.Key.Name,
                        Type = g.Key.Type,
                        Total = g.Sum(t => t.Amount),
                        Count = g.Count(),
                        AverageAmount = g.Average(t => t.Amount)
                    }),
                MonthlyTrend = transactions
                    .GroupBy(t => new { t.Date.Year, t.Date.Month, t.Type })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Type = g.Key.Type,
                        Total = g.Sum(t => t.Amount)
                    }),
                TopExpenses = transactions
                    .Where(t => t.Type == "Expense")
                    .OrderByDescending(t => t.Amount)
                    .Take(5)
                    .Select(t => new
                    {
                        t.Date,
                        t.Amount,
                        Category = t.Category.Name
                    }),
                BudgetCompliance = CalculateBudgetCompliance(transactions, accountId)
            };
        }

        private async Task<object> CalculateBudgetCompliance(List<Transaction> transactions, int accountId)
        {
            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.AccountId == accountId);

            if (budget == null)
                return null;

            var totalExpenses = transactions
                .Where(t => t.Type == "Expense")
                .Sum(t => t.Amount);

            return new
            {
                BudgetLimit = budget.Limit,
                ActualSpending = totalExpenses,
                Variance = budget.Limit - totalExpenses,
                CompliancePercentage = (totalExpenses / budget.Limit) * 100
            };
        }
    }
}