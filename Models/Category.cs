 namespace coa_Wallet.Models
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        // Add these properties
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }
        // Add navigation property for transactions
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}   