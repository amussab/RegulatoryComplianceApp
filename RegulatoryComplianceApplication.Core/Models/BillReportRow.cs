namespace RegulatoryComplianceApplication.Core.Models
{
    public class BillReportRow
    {
        public string BillName { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string AssignedUser { get; set; } = string.Empty;

        public DateOnly DueDate { get; set; }

        public string Frequency { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}