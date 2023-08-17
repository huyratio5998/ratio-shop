namespace RatioShop.Areas.Admin.Models.SiteSettings.SettingItemType
{
    public class ImageItemViewModel
    {
        public IFormFile? Icon { get; set; }
        public string? ImageSrc { get; set; }
        public string? ImageAlt { get; set; }
    }
}
