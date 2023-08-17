using RatioShop.Areas.Admin.Models.SiteSettings.SettingItemType;

namespace RatioShop.Areas.Admin.Models.SiteSettings.SettingItem
{
    public class SlideSettingItemViewModel
    {
        public ImageItemViewModel? Image { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public LinkItemViewModel? ShopNow { get; set; }
    }
}
