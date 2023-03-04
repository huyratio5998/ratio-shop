namespace RatioShop.Data.Models
{
    public class Category : BaseEntity
    {
        public string? Name { get; set; }
        public string? DisplayName { get; set; }

        List<ProductCategory> ProductCategories { get; set; }
        public int CatalogId { get; set; }
        public Catalog Catalog { get; set; }
    }
}