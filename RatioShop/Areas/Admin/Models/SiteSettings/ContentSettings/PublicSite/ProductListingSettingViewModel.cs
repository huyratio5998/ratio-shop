using RatioShop.Areas.Admin.Models.SiteSettings.ContentSettings.Common;
using RatioShop.Constants;
using RatioShop.Enums;

namespace RatioShop.Areas.Admin.Models.SiteSettings.ContentSettings.PublicSite
{
    public class ProductListingSettingViewModel : BaseSettingViewModel
    {
        public ProductListingSettingViewModel()
        {
            ProductsSorts = new Dictionary<SortingEnum, string>()
            {
                [SortingEnum.Default] = "Default",
                [SortingEnum.Oldest] = "Oldest",
                [SortingEnum.HeightoLow] = "Price: High to Low",
                [SortingEnum.LowtoHeigh] = "Price: Low to High"
            };
        }
        public int? DekstopPageSize { get; set; }
        public int? MobilePageSize { get; set; }
        public List<TextFilterSettings>? CategoriesFilter { get; set; }
        public string? PackageText { get; set; }
        public Dictionary<SortingEnum, string>? ProductsSorts { get; set; }

        public string? CategoriesFilterString { get; set; }
    }
}
