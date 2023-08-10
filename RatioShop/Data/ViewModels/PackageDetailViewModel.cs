namespace RatioShop.Data.ViewModels
{
    public class PackageDetailViewModel
    {
        public PackageDetailViewModel(PackageViewModel productPackage)
        {
            ProductPackage = productPackage;
        }

        public PackageViewModel ProductPackage { get; set; }
        public List<PackageViewModel>? OtherPackages { get; set; }              
    }
}
