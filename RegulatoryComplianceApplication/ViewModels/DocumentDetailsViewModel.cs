namespace RegulatoryComplianceApplication.Web.ViewModels
{
    public class DocumentDetailsViewModel
    {
        public int DocumentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<VersionRow> Versions { get; set; } = new();
    }

    public class VersionRow
    {
        public int VersionNumber { get; set; }
        public DateOnly IssueDate { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public bool IsCurrent { get; set; }
    }
}