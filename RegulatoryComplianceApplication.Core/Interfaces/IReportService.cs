using RegulatoryComplianceApplication.Core.Models;

public interface IReportService
{
    Task<byte[]> GenerateDocumentsPdfAsync(
        List<DocumentReportRow> documents,
        int validCount,
        int expiringSoonCount,
        int expiredCount);

    Task<byte[]> GenerateDocumentsExcelAsync(
        List<DocumentReportRow> documents);

    Task<byte[]> GenerateBillsPdfAsync(
        List<BillReportRow> bills,
        int paidCount,
        int pendingCount,
        int overdueCount);
    Task<byte[]> GenerateBillsExcelAsync(
    List<BillReportRow> bills);
}