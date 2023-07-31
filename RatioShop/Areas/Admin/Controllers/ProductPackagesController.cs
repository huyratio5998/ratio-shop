using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Data.Models;
using RatioShop.Data.ViewModels;
using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Enums;
using RatioShop.Helpers;
using RatioShop.Helpers.FileHelpers;
using RatioShop.Services.Abstract;

namespace RatioShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductPackagesController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IPackageService _packageService;
        private readonly IProductVariantService _productVariantService;
        private readonly IMapper _mapper;

        private const int pageSizeClientDesktopDefault = 12;
        private const int pageSizeClientMobileDefault = 8;

        public ProductPackagesController(IPackageService packageService, IMapper mapper, IWebHostEnvironment hostingEnvironment, IProductVariantService productVariantService)
        {
            _packageService = packageService;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
            _productVariantService = productVariantService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FilterPackage(string actionRedirect, string? code, string? name, int? page = 1)
        {
            var listFilterItems = new List<FacetFilterItem>();
            if (!string.IsNullOrWhiteSpace(code))
                listFilterItems.Add(new FacetFilterItem() { FieldName = "Code", Type = FilterType.Text.ToString(), Value = code });
            if (!string.IsNullOrWhiteSpace(name))
                listFilterItems.Add(new FacetFilterItem() { FieldName = "Name", Type = FilterType.Text.ToString(), Value = name });

            if (page <= 1) page = null;

            return RedirectToAction(actionRedirect, new { filterItems = listFilterItems.FilterItemToJson(), page = page });
        }

        public IActionResult Index(BaseSearchArgs args)
        {
            var request = _mapper.Map<BaseSearchRequest>(args);
            request.IsSelectPreviousItems = false;
            request.PageSize = CommonHelper.GetClientDevice(Request) == DeviceType.Desktop ? pageSizeClientDesktopDefault : pageSizeClientMobileDefault;
            var packages = _packageService.GetPackages(request);

            ViewBag.Area = "Admin";
            ViewBag.Controller = "Packages";
            ViewBag.Action = "Index";
            ViewBag.DetailParam = "Index";

            return View(packages);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || id == Guid.Empty) return NotFound();

            var packageViewModel = _packageService.GetPackageViewModel((Guid)id);
            packageViewModel.Image = packageViewModel.Image.ResolveProductImages().FirstOrDefault();

            if (packageViewModel == null) return NotFound();

            return View(packageViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PackageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var packageEntity = _mapper.Map<Package>(model);

                if (model.ImageFile != null)
                {
                    var uploadFileStatus = FileHelpers.UploadFile(model.ImageFile, _hostingEnvironment, "images", "packages");
                    if (uploadFileStatus) packageEntity.Image = model.ImageFile?.FileName;
                }

                await _packageService.CreatePackage(packageEntity);

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid? id, string errorMessage = "")
        {
            if (id == null || id == Guid.Empty) return NotFound();

            var packageViewModel = _packageService.GetPackageViewModel((Guid)id);

            if (packageViewModel == null) return NotFound();

            ViewBag.ErrorMessage = errorMessage;
            return View(packageViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, PackageViewModel model)
        {
            if (model == null || id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var packageEntity = _mapper.Map<Package>(model);

                if (model.ImageFile != null)
                {
                    var updateStatus = FileHelpers.UploadFile(model.ImageFile, _hostingEnvironment, "images", "packages");
                    if (updateStatus) packageEntity.Image = model.ImageFile.FileName;
                }

                _packageService.UpdatePackage(packageEntity);

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var package = _packageService.GetPackage(id.ToString());
            var packageViewModel = _mapper.Map<PackageViewModel>(package);

            if (package == null || packageViewModel == null) return NotFound();

            return View(packageViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var package = _packageService.DeletePackage(id.ToString());
            return RedirectToAction(nameof(Index));
        }

        #region PackageItem
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPackageItem(Guid variantId, Guid packageId, int itemNumber)
        {
            if (variantId == Guid.Empty || packageId == Guid.Empty || itemNumber <= 0) return NotFound();

            var updateStatus = _packageService.UpdatePackageItem(variantId, packageId, itemNumber);

            if (!updateStatus)
                return RedirectToAction("Edit", new { id = packageId, errorMessage = "Update package item failure!" });

            return RedirectToAction("Edit", new { id = packageId });
        }

        [HttpPost]
        public IActionResult RemovePackageItem(Guid id, Guid packageId)
        {
            if (id == Guid.Empty || packageId == Guid.Empty) return NotFound();

            var deleteStatus = _packageService.DeletePackageItem(id, packageId);
            if (!deleteStatus)
                return RedirectToAction("Edit", new { id = packageId, errorMessage = "Delete package item failure!" });

            return RedirectToAction("Edit", new { id = packageId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FilterProductVariant(string actionRedirect, string? name, int? page = 1)
        {
            var listFilterItems = new List<FacetFilterItem>();
            if (!string.IsNullOrWhiteSpace(name))
                listFilterItems.Add(new FacetFilterItem() { FieldName = "Name", Type = FilterType.FreeText.ToString(), Value = name });

            if (page <= 1) page = null;

            return RedirectToAction(actionRedirect, new { filterItems = listFilterItems.FilterItemToJson(), page = page });
        }

        public IActionResult AddPackageItem(Guid packageId, BaseSearchArgs args, string message = "")
        {
            if (packageId == Guid.Empty) return RedirectToAction("Index");

            var request = _mapper.Map<BaseSearchRequest>(args);
            request.IsSelectPreviousItems = false;
            request.PageSize = CommonHelper.GetClientDevice(Request) == DeviceType.Desktop ? pageSizeClientDesktopDefault : pageSizeClientMobileDefault;

            ListProductVariantViewModel listVariants = _productVariantService.GetListVariants(request);

            ViewBag.PackageId = packageId.ToString();
            ViewBag.Area = "Admin";
            ViewBag.Controller = "Packages";
            ViewBag.Action = "AddPackageItem";
            ViewBag.DetailParam = "AddPackageItem";

            return View(listVariants);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPackageItem(Guid variantId, Guid packageId, int itemNumber)
        {
            if (variantId == Guid.Empty || packageId == Guid.Empty || itemNumber <= 0) return NotFound();

            var packageItem = _packageService.GetPackageItem(variantId, packageId);
            var updateStatus = false;

            if (packageItem != null)
            {
                packageItem.ItemNumber += itemNumber;
                updateStatus = _packageService.UpdatePackageItem(packageItem);
            }
            else
            {
                updateStatus = await _packageService.CreatePackageItem(variantId, packageId, itemNumber);
            }

            if (!updateStatus)
                return View(new { packageId = packageId, message = "Update package item failure!" });

            return View(new { packageId = packageId, message = "Update package item successfully!" });
        }

        #endregion
    }
}
