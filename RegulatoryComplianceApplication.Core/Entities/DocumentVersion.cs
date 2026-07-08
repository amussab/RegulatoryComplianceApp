namespace RegulatoryComplianceApplication.Core.Entities
{
    public class DocumentVersion
    {
        public int DocumentVersionId { get; set; }
        public int DocumentId { get; set; }
        public Document Document { get; set; } = null!;
        public int VersionNumber { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public DateOnly IssueDate { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public int UploadedByUserId { get; set; }
        public User UploadedByUser { get; set; } = null!;
        public DateTime UploadedAt { get; set; }
    }
}