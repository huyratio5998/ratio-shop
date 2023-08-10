using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RatioShop.Areas.Admin.Models.SiteSettings;
using RatioShop.Areas.Admin.Models.SiteSettings.SettingItem;
using RatioShop.Constants;
using RatioShop.Data.Models;
using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Enums;
using RatioShop.Helpers;
using RatioShop.Helpers.FileHelpers;
using RatioShop.Services.Abstract;

namespace RatioShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SiteSettingsController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ISiteSettingService _siteSettingService;
        private readonly IMapper _mapper;

        private const int pageSizeClientDesktopDefault = 12;
        private const int pageSizeClientMobileDefault = 8;

        public SiteSettingsController(ISiteSettingService SiteSettingService, IMapper mapper, IWebHostEnvironment hostingEnvironment)
        {
            _siteSettingService = SiteSettingService;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FilterSiteSetting(string actionRedirect, string? name, string? settingTemplate, SiteSettingType? type, int? page = 1)
        {
            var listFilterItems = new List<FacetFilterItem>();
            if (!string.IsNullOrWhiteSpace(name))
                listFilterItems.Add(new FacetFilterItem() { FieldName = "Name", Type = FilterType.Text.ToString(), Value = name });
            if (!string.IsNullOrWhiteSpace(settingTemplate))
                listFilterItems.Add(new FacetFilterItem() { FieldName = "SettingTemplate", Type = FilterType.Text.ToString(), Value = settingTemplate });
            listFilterItems.Add(new FacetFilterItem() { FieldName = "Type", Type = FilterType.Text.ToString(), Value = type?.ToString() ?? String.Empty });
            if (page <= 1) page = null;

            return RedirectToAction(actionRedirect, new { filterItems = listFilterItems.FilterItemToJson(), page = page });
        }

        public IActionResult Index(BaseSearchArgs args)
        {
            var request = _mapper.Map<BaseSearchRequest>(args);
            request.IsSelectPreviousItems = false;
            request.PageSize = CommonHelper.GetClientDevice(Request) == DeviceType.Desktop ? pageSizeClientDesktopDefault : pageSizeClientMobileDefault;
            var siteSettings = _siteSettingService.GetSiteSettings(request);

            ViewBag.Area = "Admin";
            ViewBag.Controller = "SiteSettings";
            ViewBag.Action = "Index";
            ViewBag.DetailParam = "Index";

            return View(siteSettings);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || id == Guid.Empty) return NotFound();

            var setting = _siteSettingService.GetSetting((Guid)id);

            if (setting == null) return NotFound();

            return View(setting);
        }

        public IActionResult Create(string templateName)
        {
            var model = new SiteSettingDetailViewModel() { SettingTemplate = templateName };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SiteSettingDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var setting = await HandleSiteSetting(model);
                if (setting == null) return View(model);

                await _siteSettingService.CreateSiteSetting(setting);

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid? id, string errorMessage = "")
        {
            if (id == null || id == Guid.Empty) return NotFound();

            var setting = _siteSettingService.GetSetting((Guid)id);
            if (setting == null) return NotFound();

            ViewBag.ErrorMessage = errorMessage;
            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SiteSettingDetailViewModel model)
        {
            if (model == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var setting = await HandleSiteSetting(model);
                if (setting == null)
                {
                    ViewBag.ErrorMessage = "Error when update setting";
                    return View(model);
                }

                _siteSettingService.UpdateSiteSetting(setting);

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var SiteSetting = _siteSettingService.DeleteSiteSetting(id.ToString());
            return RedirectToAction(nameof(Index));
        }

        #region Handle each setting type
        private async Task<SiteSetting>? HandleSiteSetting(SiteSettingDetailViewModel model)
        {
            if (model == null) return null;

            // handle model...
            if (model.SettingTemplate == SiteSettings.SettingTemplates.Header || model.SettingTemplate == SiteSettings.SettingTemplates.AdminHeader)
            {
                if (model.HeaderSetting?.ShopLogo?.Icon?.Icon != null)
                {
                    var uploadStatus = await FileHelpers.UploadFile(model.HeaderSetting.ShopLogo.Icon.Icon, _hostingEnvironment, "images", "icons");
                    if (uploadStatus)
                    {
                        model.HeaderSetting.ShopLogo.Icon.ImageAlt = model.HeaderSetting.ShopLogo.Icon.Icon.FileName;
                        model.HeaderSetting.ShopLogo.Icon.ImageSrc = $"/images/icons/{model.HeaderSetting.ShopLogo.Icon.Icon.FileName}";
                    }

                    // reset image, only save image name to db
                    model.HeaderSetting.ShopLogo.Icon.Icon = null;
                }
                if (!string.IsNullOrWhiteSpace(model.HeaderSetting?.NavigationsStringValue))
                {
                    model.HeaderSetting.Navigations = JsonConvert.DeserializeObject<List<LinkItemViewModel>>(model.HeaderSetting.NavigationsStringValue);
                    model.HeaderSetting.NavigationsStringValue = null;
                }

                if (model.HeaderSetting != null)
                {
                    model.HeaderSetting.CreatedDate = model.CreatedDate;
                    model.HeaderSetting.ModifiedDate = model.ModifiedDate;
                    model.HeaderSetting.Id = model.Id;
                }
            }
            else if (model.SettingTemplate == SiteSettings.SettingTemplates.Footer || model.SettingTemplate == SiteSettings.SettingTemplates.AdminFooter)
            {
                if (!string.IsNullOrWhiteSpace(model.FooterSetting?.FooterInfosString))
                {
                    model.FooterSetting.FooterInfos = JsonConvert.DeserializeObject<List<ListItemsWithTitleViewModel>>(model.FooterSetting.FooterInfosString);
                    model.FooterSetting.FooterInfosString = null;
                }

                if (!string.IsNullOrWhiteSpace(model.FooterSetting?.PaymentSupportsString))
                {
                    model.FooterSetting.PaymentSupports = JsonConvert.DeserializeObject<List<LinkItemViewModel>>(model.FooterSetting.PaymentSupportsString);
                    model.FooterSetting.PaymentSupportsString = null;
                    model.FooterSetting.PaymentSupports?.ForEach(x =>
                    {
                        if (x.Icon != null)
                            x.Icon.Icon = null;
                    });
                }

                if(model.FooterSetting != null)
                {
                    model.FooterSetting.CreatedDate = model.CreatedDate;
                    model.FooterSetting.ModifiedDate = model.ModifiedDate;
                    model.FooterSetting.Id = model.Id;
                }
            }
            else if (model.SettingTemplate == SiteSettings.SettingTemplates.General)
            {
                if (model.GeneralSetting?.SiteLogo?.Icon != null)
                {
                    var uploadStatus = await FileHelpers.UploadFile(model.GeneralSetting.SiteLogo.Icon, _hostingEnvironment, "images", "icons");
                    if (uploadStatus)
                    {
                        model.GeneralSetting.SiteLogo.ImageAlt = model.GeneralSetting.SiteLogo.Icon.FileName;
                        model.GeneralSetting.SiteLogo.ImageSrc = $"/images/icons/{model.GeneralSetting.SiteLogo.Icon.FileName}";
                    }

                    // reset image, only save image name to db
                    model.GeneralSetting.SiteLogo.Icon = null;
                }

                if (model.GeneralSetting != null)
                {
                    model.GeneralSetting.CreatedDate = model.CreatedDate;
                    model.GeneralSetting.ModifiedDate = model.ModifiedDate;
                    model.GeneralSetting.Id = model.Id;
                }
            }
            else if (model.SettingTemplate == SiteSettings.SettingTemplates.SEO)
            {
                if (model.SEOSetting != null)
                {
                    model.SEOSetting.CreatedDate = model.CreatedDate;
                    model.SEOSetting.ModifiedDate = model.ModifiedDate;
                    model.SEOSetting.Id = model.Id;
                }
            }
            else if (model.SettingTemplate == SiteSettings.SettingTemplates.Slide)
            {
                if (!string.IsNullOrWhiteSpace(model.SlideSetting?.BannersString))
                {
                    model.SlideSetting.Banners = JsonConvert.DeserializeObject<List<SlideSettingItemViewModel>>(model.SlideSetting.BannersString);
                    model.SlideSetting.BannersString = null;
                    model.SlideSetting.Banners?.ForEach(x =>
                    {
                        if (x.Image?.Icon != null)
                            x.Image.Icon = null;
                    });
                }

                if (model.SlideSetting != null)
                {
                    model.SlideSetting.CreatedDate = model.CreatedDate;
                    model.SlideSetting.ModifiedDate = model.ModifiedDate;
                    model.SlideSetting.Id = model.Id;
                }
            }
            else if (model.SettingTemplate == SiteSettings.SettingTemplates.ProductListing)
            {
                if (!string.IsNullOrWhiteSpace(model.ProductListingSetting?.CategoriesFilterString))
                {
                    model.ProductListingSetting.CategoriesFilter = JsonConvert.DeserializeObject<List<TextFilterSettings>>(model.ProductListingSetting.CategoriesFilterString);
                    model.ProductListingSetting.CategoriesFilterString = null;
                }

                if (model.ProductListingSetting != null)
                {
                    model.ProductListingSetting.CreatedDate = model.CreatedDate;
                    model.ProductListingSetting.ModifiedDate = model.ModifiedDate;
                    model.ProductListingSetting.Id = model.Id;
                }
            }
            else if (model.SettingTemplate == SiteSettings.SettingTemplates.ProductDetail)
            {
                if (model.ProductDetailSetting != null)
                {
                    model.ProductDetailSetting.CreatedDate = model.CreatedDate;
                    model.ProductDetailSetting.ModifiedDate = model.ModifiedDate;
                    model.ProductDetailSetting.Id = model.Id;
                }
            }
            else if (model.SettingTemplate == SiteSettings.SettingTemplates.AdminGeneral)
            {
                if (model.AdminGeneralSetting?.SiteLogo?.Icon != null)
                {
                    var uploadStatus = await FileHelpers.UploadFile(model.AdminGeneralSetting.SiteLogo.Icon, _hostingEnvironment, "images", "icons");
                    if (uploadStatus)
                    {
                        model.AdminGeneralSetting.SiteLogo.ImageAlt = model.AdminGeneralSetting.SiteLogo.Icon.FileName;
                        model.AdminGeneralSetting.SiteLogo.ImageSrc = $"/images/icons/{model.AdminGeneralSetting.SiteLogo.Icon.FileName}";
                    }

                    // reset image, only save image name to db
                    model.AdminGeneralSetting.SiteLogo.Icon = null;
                }

                if (model.AdminGeneralSetting != null)
                {
                    model.AdminGeneralSetting.CreatedDate = model.CreatedDate;
                    model.AdminGeneralSetting.ModifiedDate = model.ModifiedDate;
                    model.AdminGeneralSetting.Id = model.Id;
                }
            }

            var siteSetting = _mapper.Map<SiteSetting>(model);

            return siteSetting;
        }

        #endregion
    }
}
