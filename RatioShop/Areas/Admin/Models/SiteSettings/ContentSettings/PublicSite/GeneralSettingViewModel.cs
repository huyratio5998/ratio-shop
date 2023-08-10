﻿using RatioShop.Areas.Admin.Models.SiteSettings.ContentSettings.Common;
using RatioShop.Areas.Admin.Models.SiteSettings.SettingItemType;

namespace RatioShop.Areas.Admin.Models.SiteSettings.ContentSettings.PublicSite
{
    public class GeneralSettingViewModel: BaseSettingViewModel
    {
        public string SiteName { get; set; }
        public string HomePageUrl { get; set; }
        public ImageItemViewModel? SiteLogo { get; set; }
    }
}
