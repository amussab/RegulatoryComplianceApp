namespace RegulatoryComplianceApplication.Core.Entities
{
    public class DocumentType
    {
        public int DocumentTypeId { get; set; }

        public string TypeName { get; set; } = string.Empty;

        public bool IsExpirable { get; set; } = true;

        public bool IsDeleted { get; set; }

        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}