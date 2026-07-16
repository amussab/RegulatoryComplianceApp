namespace RegulatoryComplianceApplication.Core.Entities
{
    public enum BillFrequency
    {
        OneTime,
        Monthly,
        Quarterly,
        Yearly
    }

    public enum BillStatus
    {
        Pending,
        Paid,
        Overdue
    }

    public class Bill
    {
        public int BillId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string BillName { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateOnly DueDate { get; set; }

        public BillFrequency Frequency { get; set; }

        public bool IsRecurring { get; set; }

        public BillStatus Status { get; set; }

        public string? AttachmentPath { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}