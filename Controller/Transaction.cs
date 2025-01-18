using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using coa_Wallet.Models;

namespace coa_wallet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddTransaction([FromBody] Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return Ok(transaction);
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var transactions = await _context.Transactions
                .Where(t => (!startDate.HasValue || t.Date >= startDate) && 
                           (!endDate.HasValue || t.Date <= endDate))
                .ToListAsync();

            return Ok(transactions);
        }
    }
}