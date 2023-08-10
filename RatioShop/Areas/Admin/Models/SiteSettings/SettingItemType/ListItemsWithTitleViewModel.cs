using RatioShop.Areas.Admin.Models.SiteSettings.SettingItem;

namespace RatioShop.Areas.Admin.Models.SiteSettings
{
    public class ListItemsWithTitleViewModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool ItemDisplayInline { get; set; }
        public List<LinkItemViewModel>? Items { get; set; }
    }
}
