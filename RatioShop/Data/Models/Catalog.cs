namespace RatioShop.Data.Models
{
    public class Catalog : BaseEntity
    {
        public string? Name { get; set; }
        public string? DisplayName { get; set; }

        List<Category> Categories { get; set; }
    }
}