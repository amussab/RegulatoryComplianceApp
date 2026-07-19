using RegulatoryComplianceApplication.Core.Models;

namespace RegulatoryComplianceApplication.Core.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> GenerateDocumentsPdfAsync(
            List<DocumentReportRow> documents,
            int validCount,
            int expiringSoonCount,
            int expiredCount);
    }
}