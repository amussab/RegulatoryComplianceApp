namespace RegulatoryComplianceApplication.Core.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int DocumentId { get; set; }
        public Document Document { get; set; } = null!;
        public int RecipientUserId { get; set; }
        public User RecipientUser { get; set; } = null!;
        public string Channel { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
    }
}