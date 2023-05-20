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
    }
}
