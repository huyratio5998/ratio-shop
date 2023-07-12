using Microsoft.AspNetCore.Authentication;
using RatioShop.Constants;

namespace RatioShop.Data.ViewModels.Layout
{
    public class LayoutSettingsViewModel : ILayoutSettingsViewModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;        

        public LayoutSettingsViewModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;            
        }

        public string StoreName => CommonConstant.StoreName;

        public string StoreIcon => "/images/icons/logo-01.png";

        public string CurrentPath()
        {
            return _httpContextAccessor.HttpContext?.Request?.Path.ToString() ?? String.Empty;
        }        

        public FooterSettingsViewModel FooterSettings()
        {
            return new FooterSettingsViewModel();
        }

        public HeaderSettingsViewModel HeaderSettings()
        {
            var currentPath = CurrentPath();
            var headerSettings = new HeaderSettingsViewModel()
            {
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
