using RatioShop.Enums;

namespace RatioShop.Data.Models
{
    public class SiteSetting : BaseProduct
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string SettingTemplate { get; set; }
        public SiteSettingType Type { get; set; }
        public bool IsActive { get; set; }
    }
}
