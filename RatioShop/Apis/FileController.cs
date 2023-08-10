using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RatioShop.Helpers.FileHelpers;

namespace RatioShop.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FileController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Route("saveFiles")]
        public async Task<bool> SaveFiles(List<IFormFile>? files, string? folderName1 = "", string? folderName2 = "", string? folderName3 = "", string? folderName4 = "")
        {
            if (files == null || !files.Any()) return false;
            folderName1 = folderName1 ?? string.Empty;
            folderName2 = folderName2 ?? string.Empty;
            folderName3 = folderName3 ?? string.Empty;
            folderName4 = folderName4 ?? string.Empty;  
            
            return await FileHelpers.UploadFiles(files, _hostingEnvironment, folderName1, folderName2, folderName3, folderName4);
        }

        [HttpPost]
        [Route("saveFile")]
        public async Task<bool> SaveFile(IFormFile? file, string? folderName1 = "", string? folderName2 = "", string? folderName3 = "", string? folderName4 = "")
        {
            if (file == null) return false;
            folderName1 = folderName1 ?? string.Empty;
            folderName2 = folderName2 ?? string.Empty;
            folderName3 = folderName3 ?? string.Empty;
            folderName4 = folderName4 ?? string.Empty;

            return await FileHelpers.UploadFile(file, _hostingEnvironment, folderName1, folderName2, folderName3, folderName4);
        }
    }
}
