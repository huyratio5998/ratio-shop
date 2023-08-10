using RatioShop.Areas.Admin.Models.SiteSettings.SettingItem;

namespace RatioShop.Areas.Admin.Models.SiteSettings.ContentSettings.Admin
{
    public class AdminFooterSettingViewModel
    {
        public List<ListItemsWithTitleViewModel>? FooterInfos { get; set; }
        public List<LinkItemViewModel>? PaymentSupports { get; set; }
        public string Coppyright { get; set; }
    }
}
