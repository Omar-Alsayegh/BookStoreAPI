using Microsoft.AspNetCore.Http;
namespace BookStoreApi.Services.FileStorage
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string subFolder);
        void DeleteFile(string filePath);
        //  string GetFilePath(string fileName, string subFolder);
        string GetPhysicalPath(string fileUrl);
    }
}
