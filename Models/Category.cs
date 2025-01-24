// Models/Category.cs
 namespace coa_Wallet.Models
{
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ParentCategoryId { get; set; }
    public int AccountId { get; set; }
    public Category? ParentCategory { get; set; }
    public Account Account { get; set; }
}

// DTOs/HierarchicalCategory.cs
public class HierarchicalCategory
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ParentCategoryId { get; set; }
    public List<HierarchicalCategory> Children { get; set; } = new List<HierarchicalCategory>();
}
}