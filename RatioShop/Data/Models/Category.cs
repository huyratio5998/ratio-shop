namespace RatioShop.Data.Models
{
    public class Category : BaseEntity
    {
        public string? Name { get; set; }
        public string? DisplayName { get; set; }

        List<ProductCategory>? ProductCategories { get; set; }
        public int? ParentId { get; set; }
        public Category? ParentCategory { get; set; }

        public virtual ICollection<Category> Children { get; set; }
    }
}