using Newtonsoft.Json;
using RatioShop.Areas.Admin.Models.SiteSettings;

namespace RatioShop.Helpers.SiteSettingsHelper
{
    public static class MappingSettingTypeHelper
    {
        public static string MappingSettingValue(SiteSettingDetailViewModel settingDetail)
        {
            var result = string.Empty;
            if (settingDetail.Type == Enums.SiteSettingType.ItemSetting) return settingDetail.SingleSetting ?? string.Empty;

            switch (settingDetail.SettingTemplate)
            {
                case Constants.SiteSettings.SettingTemplates.Header:
                    result = JsonConvert.SerializeObject(settingDetail.HeaderSetting);
                    break;
                case Constants.SiteSettings.SettingTemplates.Footer:
                    result = JsonConvert.SerializeObject(settingDetail.FooterSetting);
                    break;
                case Constants.SiteSettings.SettingTemplates.AdminHeader:
                    result = JsonConvert.SerializeObject(settingDetail.HeaderSetting);
                    break;
                case Constants.SiteSettings.SettingTemplates.AdminFooter:
                    result = JsonConvert.SerializeObject(settingDetail.FooterSetting);
                    break;
                case Constants.SiteSettings.SettingTemplates.General:
                    result = JsonConvert.SerializeObject(settingDetail.GeneralSetting);
                    break;
                case Constants.SiteSettings.SettingTemplates.SEO:
                    result = JsonConvert.SerializeObject(settingDetail.SEOSetting);
                    break;
                case Constants.SiteSettings.SettingTemplates.Slide:
                    result = JsonConvert.SerializeObject(settingDetail.SlideSetting);
                    break;
                case Constants.SiteSettings.SettingTemplates.ProductListing:
                    result = JsonConvert.SerializeObject(settingDetail.ProductListingSetting);
                    break;
                case Constants.SiteSettings.SettingTemplates.ProductDetail:
                    result = JsonConvert.SerializeObject(settingDetail.ProductDetailSetting);
                    break;
                case Constants.SiteSettings.SettingTemplates.AdminGeneral:
                    result = JsonConvert.SerializeObject(settingDetail.AdminGeneralSetting);
                    break;
            }

            return result;
        }
    }
}
