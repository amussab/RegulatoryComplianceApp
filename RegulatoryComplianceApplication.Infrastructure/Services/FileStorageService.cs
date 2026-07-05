using Microsoft.Extensions.Configuration;
using RegulatoryComplianceApplication.Core.Interfaces;

namespace RegulatoryComplianceApplication.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _basePath;

        public FileStorageService(IConfiguration configuration)
        {
            _basePath = configuration["FileStorage:BasePath"] ?? "App_Data/Documents";
            Directory.CreateDirectory(_basePath);
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName)
        {
            var uniqueName = $"{Guid.NewGuid()}_{fileName}";
            var fullPath = Path.Combine(_basePath, uniqueName);

            using (var output = new FileStream(fullPath, FileMode.Create))
            {
                await fileStream.CopyToAsync(output);
            }

            return fullPath;
        }

        public Task<Stream> GetFileAsync(string filePath)
        {
            Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return Task.FromResult(stream);
        }

        public void DeleteFile(string filePath)
        {
            if (File.Exists(filePath)) File.Delete(filePath);
        }
    }
}