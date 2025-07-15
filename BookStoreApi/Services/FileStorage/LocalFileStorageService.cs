using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
namespace BookStoreApi.Services.FileStorage
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _uploadsFolder;
        private readonly string _requestPath;
        public LocalFileStorageService(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _uploadsFolder = configuration["FileStorageSettings:UploadsFolder"] ?? "Uploads";
            _requestPath = configuration["FileStorage:RequestPath"] ?? "/Uploads";
        }

        public string GetPhysicalPath(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
            {
                return null;
            }

            string relativePathWithinUploads = fileUrl;
            if (fileUrl.StartsWith(_requestPath, StringComparison.OrdinalIgnoreCase))
            {
                relativePathWithinUploads = fileUrl.Substring(_requestPath.Length);
            }

            relativePathWithinUploads = relativePathWithinUploads.TrimStart('/');

            var absolutePath = Path.Combine(_webHostEnvironment.ContentRootPath, _uploadsFolder, relativePathWithinUploads);

            return absolutePath;
        }

        void IFileStorageService.DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            string relativePath = filePath.TrimStart('/');
            string fullPath = Path.Combine(_webHostEnvironment.ContentRootPath, relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        //string IFileStorageService.GetFilePath(string fileName, string subFolder)
        //{
        //    return $"/{_uploadsFolder}/{subFolder}/{fileName}";
        //}

        async Task<string> IFileStorageService.SaveFileAsync(IFormFile file, string subFolder)
        {
            if (file == null || file.Length == 0)
            {
                return string.Empty;
            }

            // Ensure the uploads directory exists
            string uploadsRootPath = Path.Combine(_webHostEnvironment.ContentRootPath, _uploadsFolder);
            string targetFolder = Path.Combine(uploadsRootPath, subFolder);

            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            // Generate a unique file name to prevent conflicts
            string extension = Path.GetExtension(file.FileName);
            string uniqueFileName = Guid.NewGuid().ToString() + extension;
            string filePath = Path.Combine(targetFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{_uploadsFolder}/{subFolder}/{uniqueFileName}";
        }

    }
}
