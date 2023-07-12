using RatioShop.Data.Models;

namespace RatioShop.Data.ViewModels
{
    public class ProductViewModel
    {
        public ProductViewModel()
        {

        }
        public ProductViewModel(Product product)
        {
            Product = product;
        }
        public Product Product { get; set; }
        public string? ProductImageName { get; set; }
        public IFormFile? ProductImage { get; set; }
        public IEnumerable<Category>? ProductCategories { get; set; }
        public IEnumerable<Category>? AvailableCategories { get; set; }
        public IDictionary<int,string>? AvailableStocks { get; set; }
        public bool IsSoldOnline { get { return (Product != null && Product.Variants != null && Product.Variants.Any(x => x.Price != null && x.Price != decimal.Zero)); } }
        public ProductVariant? SelectedVariant { get; set; }
    }
}
