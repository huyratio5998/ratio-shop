using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Services.Abstract;

namespace RatioShop.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;
        private readonly IMapper _mapper;

        public PackageController(IPackageService packageService, IMapper mapper)
        {
            _packageService = packageService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("detail/{packageId:guid}")]
        public IActionResult GetPackageDetail([FromRoute] Guid packageId)
        {
            if (packageId == Guid.Empty) return NotFound();

            var package = _packageService.GetPackageViewModel(packageId);            

            return Ok(package);
        }
    }
}
