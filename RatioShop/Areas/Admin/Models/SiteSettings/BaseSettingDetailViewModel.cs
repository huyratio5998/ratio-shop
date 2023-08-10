using RatioShop.Enums;

namespace RatioShop.Areas.Admin.Models.SiteSettings
{
    public class BaseSettingDetailViewModel
    {
        public Guid Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Name { get; set; }
        public string SettingTemplate { get; set; }
        public SiteSettingType Type { get; set; }
        public bool IsActive { get; set; }
    }
}
