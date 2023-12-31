﻿namespace RatioShop.Helpers.FileHelpers
{
    public static class FileHelpers
    {
        public static async Task<bool> UploadFile(IFormFile file, IWebHostEnvironment environment, string folderName1 = "", string folderName2 = "", string folderName3 = "", string folderName4 = "")
        {
            try
            {
                if (file == null) return false;

                var wwwroot = environment.WebRootPath;
                var pathFolder = Path.Combine(wwwroot, folderName1, folderName2, folderName3, folderName4);

                // Determine whether the directory exists.
                if (!Directory.Exists(pathFolder))
                {
                    Directory.CreateDirectory(pathFolder);
                }

                var path = Path.Combine(pathFolder, file.FileName);
                if (File.Exists(path)) return true;

                await file.CopyToAsync(new FileStream(path, FileMode.Create));

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static async Task<bool> UploadFiles(List<IFormFile> files, IWebHostEnvironment environment, string folderName1 = "", string folderName2 = "", string folderName3 = "", string folderName4 = "")
        {
            try
            {
                if (files == null || !files.Any()) return false;

                var wwwroot = environment.WebRootPath;
                var pathFolder = Path.Combine(wwwroot, folderName1, folderName2, folderName3, folderName4);

                // Determine whether the directory exists.
                if (!Directory.Exists(pathFolder))
                {
                    Directory.CreateDirectory(pathFolder);
                }

                foreach (var file in files)
                {
                    var path = Path.Combine(pathFolder, file.FileName);
                    if (File.Exists(path)) continue;

                    await file.CopyToAsync(new FileStream(path, FileMode.Create));
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
