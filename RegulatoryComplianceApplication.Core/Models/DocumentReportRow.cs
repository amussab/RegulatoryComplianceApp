namespace RegulatoryComplianceApplication.Core.Models
{
    public class DocumentReportRow
    {
        public string Title { get; set; } = string.Empty;

        public string DocumentNumber { get; set; } = string.Empty;

        public string ResponsibleUser { get; set; } = string.Empty;

        public DateOnly? ExpiryDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}