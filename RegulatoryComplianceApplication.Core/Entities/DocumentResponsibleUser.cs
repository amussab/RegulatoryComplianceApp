namespace RegulatoryComplianceApplication.Core.Entities
{
    public class DocumentResponsibleUser
    {
        public int DocumentId { get; set; }
        public Document Document { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public bool IsUploader { get; set; }
        public int AddedByUserId { get; set; }
        public User AddedByUser { get; set; } = null!;
        public DateTime AddedAt { get; set; }
    }
}