using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Enums;

namespace RatioShop.Constants
{
    public static class SiteSettings
    {
        public static bool EnableTrackProduct = true;
        public static bool EnableTrackCoupon = true;

        public static Dictionary<SortingEnum, string> ProductsSorts = new Dictionary<SortingEnum, string>()
        {
            [SortingEnum.Default] = "Default",
            [SortingEnum.Oldest] = "Oldest",
            [SortingEnum.HeightoLow] = "Price: High to Low",
            [SortingEnum.LowtoHeigh] = "Price: Low to High"
        };

        public static List<TextFilterSettings> CategoriesFilter = new List<TextFilterSettings>()
        {
            new TextFilterSettings()
            {
                Value = null,
                DisplayName ="All Products",
                Position = 0
            },
            new TextFilterSettings()
            {
                Value = "9",
                DisplayName ="Samsung",
                Position = 1
            },
            new TextFilterSettings()
            {
                Value = "10",
                DisplayName ="Iphone",
                Position = 2
            },
            new TextFilterSettings()
            {
                Value = "14",
                DisplayName ="Foods",
                Position = 3
            },
            new TextFilterSettings()
            {
                Value = "16",
                DisplayName ="Drinks",
                Position = 4
            },
            new TextFilterSettings()
            {
                Value = "17",
                DisplayName ="Book",
                Position = 5
            },
        };
    }
    public class TextFilterSettings
    {
        public string? Value { get; set; }
        public string DisplayName { get; set; }
        public int Position { get; set; }
    }
}
