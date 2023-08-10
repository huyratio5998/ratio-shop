using RatioShop.Areas.Admin.Models.SiteSettings.SettingItemType;

namespace RatioShop.Areas.Admin.Models.SiteSettings.SettingItem
{
    public class LinkItemViewModel
    {
        public ImageItemViewModel? Icon { get; set; }        
        public string? Text { get; set; }
        public string Url { get; set; }
    }
}
