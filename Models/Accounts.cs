namespace coa_Wallet.Models
{
    public class Account
    {
        public int Id { get; set; }
        
        
        public required string Name { get; set; } = string.Empty;
        
        public required decimal Balance { get; set; }

        public string UserId { get; set; } = string.Empty;
        
        public User? User { get; set; }
    }
}