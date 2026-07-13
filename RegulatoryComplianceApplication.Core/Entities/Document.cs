namespace RegulatoryComplianceApplication.Core.Entities
{
    public class Document
    {
        public int DocumentId { get; set; }
        public int DocumentTypeId { get; set; }
        public DocumentType DocumentType { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public int? CurrentVersionId { get; set; }
        public DocumentVersion? CurrentVersion { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<DocumentVersion> Versions { get; set; } = new List<DocumentVersion>();
        public ICollection<DocumentResponsibleUser> ResponsibleUsers { get; set; } = new List<DocumentResponsibleUser>();
    }
}