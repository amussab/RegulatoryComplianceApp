namespace RegulatoryComplianceApplication.Core.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(Stream fileStream, string fileName);
        Task<Stream> GetFileAsync(string filePath);
        void DeleteFile(string filePath);
    }
}