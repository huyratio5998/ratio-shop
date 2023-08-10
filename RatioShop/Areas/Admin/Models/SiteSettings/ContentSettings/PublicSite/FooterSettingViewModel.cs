using RatioShop.Areas.Admin.Models.SiteSettings.ContentSettings.Common;
using RatioShop.Areas.Admin.Models.SiteSettings.SettingItem;

namespace RatioShop.Areas.Admin.Models.SiteSettings.ContentSettings.PublicSite
{
    public class FooterSettingViewModel: BaseSettingViewModel
    {
        public List<ListItemsWithTitleViewModel>? FooterInfos { get; set; }
        public List<LinkItemViewModel>? PaymentSupports { get; set; }
        public string? Coppyright { get; set; }

        public string? FooterInfosString { get; set; }
        public string? PaymentSupportsString { get; set; }
    }
}
