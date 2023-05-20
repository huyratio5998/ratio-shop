﻿namespace RatioShop.Helpers.FileHelpers
{
    public static class FileHelpers
    {
        public static bool UploadFile(IFormFile file, IWebHostEnvironment environment, string folderName1 = "", string folderName2 = "", string folderName3 = "", string folderName4 = "")
        {
            if(file == null) return false;

            var wwwroot = environment.WebRootPath;             
            var pathFolder = Path.Combine(wwwroot, folderName1, folderName2, folderName3, folderName4);
            var path = Path.Combine(pathFolder, file.FileName);
            if (File.Exists(path)) return false;

            file.CopyTo(new FileStream(path, FileMode.Create));

            return true;
        }
    }
}