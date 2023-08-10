using RatioShop.Constants;

namespace RatioShop.Data.ViewModels.SearchViewModel
{
    public class FilterSettings
    {
        public List<TextFilterSettings>? CategoryFilter { get; set; }
        public Dictionary<decimal,decimal?>? PriceRangeFilter { get; set; }
        public bool IsPackageView { get; set; }
    }
}
