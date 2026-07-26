using RegulatoryComplianceApplication.Core.Entities;

namespace RegulatoryComplianceApplication.Core.Interfaces
{
    public interface IDocumentService
    {
        Task<Document?> GetByIdAsync(int documentId);
        Task<IEnumerable<Document>> GetAllAsync();
        Task<IEnumerable<Document>> GetExpiredAsync();

        Task<Document> CreateAsync(
            Document document,
            DocumentVersion firstVersion,
            int uploadedByUserId,
            int responsibleUserId);

        Task<DocumentVersion> RenewAsync(int documentId, DocumentVersion newVersion, int uploadedByUserId);

        Task<IEnumerable<Document>> GetExpiringSoonAsync(int daysThreshold);
        Task SoftDeleteAsync(int documentId, int deletedByUserId);
        Task UpdateAsync(
            Document document,
            DocumentVersion currentVersion,
            int responsibleUserId,
            int editedByUserId);
    }
}