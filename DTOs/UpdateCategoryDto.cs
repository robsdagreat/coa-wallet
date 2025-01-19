namespace coa_Wallet.DTOs
{
    public class UpdateCategoryDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}