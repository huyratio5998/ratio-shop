using Microsoft.AspNetCore.Authentication;
using RatioShop.Areas.Admin.Models.SiteSettings;
using RatioShop.Constants;
using RatioShop.Services.Abstract;

namespace RatioShop.Data.ViewModels.Layout
{
    public class LayoutSettingsViewModel : ILayoutSettingsViewModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;        
        private readonly ISiteSettingService _siteSettingService;

        public LayoutSettingsViewModel(IHttpContextAccessor httpContextAccessor, ISiteSettingService siteSettingService)
        {
            _httpContextAccessor = httpContextAccessor;
            _siteSettingService = siteSettingService;
            this.SiteSettings = _siteSettingService.GetSiteSetting()?.Result;
        }
        private SiteSettingViewModel? SiteSettings;

        public string StoreName => SiteSettings?.GeneralSetting?.SiteName ?? CommonConstant.StoreName;

        public string StoreIcon => SiteSettings?.HeaderSetting?.ShopLogo?.Icon?.ImageSrc ?? "/images/icons/logo-01.png";

        public string StoreLogo => SiteSettings?.GeneralSetting?.SiteLogo?.ImageSrc ?? "/images/icons/favicon.png";

        SiteSettingViewModel ILayoutSettingsViewModel.SiteSettings => this.SiteSettings;

        public string CurrentPath()
        {
            return _httpContextAccessor.HttpContext?.Request?.Path.ToString() ?? String.Empty;
        }        

        public FooterSettingsViewModel FooterSettings()
        {
            return new FooterSettingsViewModel
            {
                FooterSetting = SiteSettings?.FooterSetting,
            };
        }

        public HeaderSettingsViewModel HeaderSettings()
        {
            var currentPath = CurrentPath();
            var headerSettings = new HeaderSettingsViewModel()
            {
                HeaderSetting = SiteSettings?.HeaderSetting,
                HeaderSlides = SiteSettings?.HeaderSlides,
                IsHideSilder = HideSliderByPath(currentPath)
            };
            return headerSettings;
        }       

        private bool HideSliderByPath(string path)
        {
            var isHide = false;
            switch (path)
            {
                case
                    var _ when path.Contains("Cart/CartDetail", StringComparison.OrdinalIgnoreCase):
                    isHide = true;
                    break;
                case
                    var _ when path.Contains("myaccount", StringComparison.OrdinalIgnoreCase):
                    isHide = true;
                    break;
                case
                    var _ when path.Contains("Checkout", StringComparison.OrdinalIgnoreCase):
                    isHide = true;
                    break;
                default:
                    break;
            }
            return isHide;
        }
        private bool HideRegisterPopup(string path)
        {
            var isHide = false;
            switch (path)
            {              
                case
                    var _ when path.Contains("myaccount", StringComparison.OrdinalIgnoreCase):
                    isHide = true;
                    break;                
                default:
                    break;
            }
            return isHide;
        }

        public CommonSettingsViewModel CommonSettings()
        {
            var currentPath = CurrentPath();
            var commonSettings = new CommonSettingsViewModel
            {
                IsHideRegisterPopup = HideRegisterPopup(currentPath)
            };
            return commonSettings;
        }        
    }
}
