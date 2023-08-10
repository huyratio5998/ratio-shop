using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Data.ViewModels;
using RatioShop.Services.Abstract;

namespace RatioShop.Features
{
    [Route("packages")]
    public class ProductPackagesController : Controller
    {
        private readonly IPackageService _packageService;
        private readonly IMapper _mapper;

        public ProductPackagesController(IPackageService packageService, IMapper mapper)
        {
            _packageService = packageService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [Route("detail/{id:guid}")]
        public async Task<IActionResult> ProductPackageDetail(Guid? id)
        {
            if (id == null) return NotFound();

            var packageViewModel = _packageService.GetPackageViewModel((Guid)id);
            var otherPackages = _packageService.GetPackages().Where(x => x.Id != id);
            //product.BreadCrumbs = _commonService.GetBreadCrumbsByProductId((Guid)id);

            if (packageViewModel == null) return NotFound();

            var result = new PackageDetailViewModel(packageViewModel);

            if (otherPackages != null && otherPackages.Any())
            {
                var otherPackagesViewModel = _mapper.Map<List<PackageViewModel>>(otherPackages);
                otherPackagesViewModel.ForEach(x => x = _packageService.GetAdditionPackageInfoViewModel(x));

                result.OtherPackages = otherPackagesViewModel;
            }

            return View("~/Views/ProductPackages/ProductPackageDetail.cshtml", result);
        }
    }
}
