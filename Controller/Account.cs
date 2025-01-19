using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using coa_Wallet.Models;

namespace coa_Wallet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] Account account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get the user ID from the JWT token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            account.UserId = userId;
            account.User = null; // Don't try to create a new user

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return Ok(account); // Changed to return Ok instead of CreatedAtAction
        }

        [HttpGet("{id}")]
public async Task<ActionResult<Account>> GetAccount(int id)
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId))
    {
        return Unauthorized();
    }

    var account = await _context.Accounts
        .Where(a => a.UserId == userId && a.Id == id)
        .FirstOrDefaultAsync();

    if (account == null)
    {
        return NotFound($"Account with ID {id} not found.");
    }

    return account;
}

[HttpGet]
public async Task<ActionResult<IEnumerable<Account>>> GetAllAccounts()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId))
    {
        return Unauthorized();
    }

    var accounts = await _context.Accounts
        .Where(a => a.UserId == userId)
        .ToListAsync();
        
    return Ok(accounts);
}

}
}