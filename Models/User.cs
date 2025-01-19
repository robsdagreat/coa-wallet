using Microsoft.AspNetCore.Identity;
namespace coa_Wallet.Models
{
    public class User : IdentityUser
    {
        public string? Role { get; set; }
        public ICollection<Account> Accounts { get; set; }
    }
}
