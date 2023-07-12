namespace RatioShop.Data.ViewModels
{
    public class UpdateProductAdditionalInformationRequest
    {
        public Guid ProductId { get; set; }
        public List<ProductVariantRequestViewModel> Variants { get; set; }
        public List<string> RemoveVariants { get; set; }
        public List<ProductCategoryRequestViewModel> ProductCategories { get; set; }
        public List<int> RemoveProductCategories { get; set; }
    }
}
