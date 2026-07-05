using RegulatoryComplianceApplication.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RegulatoryComplianceApplication.Core.Interfaces
{
    public interface IDocumentService
    {
        Task<Document?> GetByIdAsync(int documentId);
        Task<IEnumerable<Document>> GetAllAsync();
        Task<Document> CreateAsync(Document document, DocumentVersion firstVersion, int uploadedByUserId);
        Task<DocumentVersion> RenewAsync(int documentId, DocumentVersion newVersion, int uploadedByUserId);
        Task<IEnumerable<Document>> GetExpiringSoonAsync(int daysThreshold);
    }
}
