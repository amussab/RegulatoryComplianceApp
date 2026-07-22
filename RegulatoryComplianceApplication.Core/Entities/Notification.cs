namespace RegulatoryComplianceApplication.Core.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }

        // Optional Document
        public int? DocumentId { get; set; }
        public Document? Document { get; set; }

        // Optional Bill
        public int? BillId { get; set; }
        public Bill? Bill { get; set; }

        public int RecipientUserId { get; set; }
        public User RecipientUser { get; set; } = null!;

        public string Channel { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? SentAt { get; set; }
    }
}