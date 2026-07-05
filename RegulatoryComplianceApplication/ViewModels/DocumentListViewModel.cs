namespace RegulatoryComplianceApplication.Web.ViewModels
{
    public class DocumentListViewModel
    {
        public int DocumentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string DocumentTypeName { get; set; } = string.Empty;
        public DateOnly? ExpiryDate { get; set; }
        public string Status { get; set; } = string.Empty; // "Valid" | "Expiring Soon" | "Expired"
    }
}