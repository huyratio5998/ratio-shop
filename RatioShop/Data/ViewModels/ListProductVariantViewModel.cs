namespace RatioShop.Data.ViewModels
{
    public class ListProductVariantViewModel : BaseListingPageViewModel
    {
        public ListProductVariantViewModel()
        {
            if (ProductVariants == null) ProductVariants = new List<ProductVariantViewModel>();
        }

        public IEnumerable<ProductVariantViewModel> ProductVariants { get; set; }
    }
}
