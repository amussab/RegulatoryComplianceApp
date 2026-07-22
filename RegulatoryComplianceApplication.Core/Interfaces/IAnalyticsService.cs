using RegulatoryComplianceApplication.Core.Entities;
using RegulatoryComplianceApplication.Core.Models;

namespace RegulatoryComplianceApplication.Core.Interfaces
{
    public interface IAnalyticsService
    {
        Task<ComplianceAnalyticsResult> AnalyzeAsync(
            IEnumerable<Document> documents,
            IEnumerable<Bill> bills);
    }
}