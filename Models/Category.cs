namespace coa_Wallet.Models
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
    }
}